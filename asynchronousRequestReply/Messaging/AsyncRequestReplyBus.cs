using System.Collections.Concurrent;
using System.Threading.Channels;

namespace asynchronousRequestReply.Messaging;

public sealed class AsyncRequestReplyBus : IAsyncDisposable
{
    private readonly Channel<RequestMessage> _requestChannel;
    private readonly ConcurrentDictionary<Guid, TaskCompletionSource<ResponseMessage>> _pendingRequests = new();

    public AsyncRequestReplyBus()
    {
        _requestChannel = Channel.CreateUnbounded<RequestMessage>(new UnboundedChannelOptions
        {
            SingleReader = false,
            SingleWriter = false
        });
    }

    public ChannelReader<RequestMessage> Requests => _requestChannel.Reader;

    public async Task<ResponseMessage> SendRequestAsync(string payload, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(payload);

        var correlationId = Guid.NewGuid();
        var createdAt = DateTimeOffset.UtcNow;
        var request = new RequestMessage(correlationId, payload, createdAt);

        var taskCompletionSource = new TaskCompletionSource<ResponseMessage>(TaskCreationOptions.RunContinuationsAsynchronously);

        if (!_pendingRequests.TryAdd(correlationId, taskCompletionSource))
        {
            throw new InvalidOperationException("Unable to queue request; correlation identifier already exists.");
        }

        CancellationTokenRegistration? registration = null;
        if (cancellationToken.CanBeCanceled)
        {
            registration = cancellationToken.Register(static state =>
            {
                var (bus, correlation) = ((AsyncRequestReplyBus bus, Guid correlation))state!;
                if (bus._pendingRequests.TryRemove(correlation, out var pending))
                {
                    pending.TrySetCanceled();
                }
            }, (this, correlationId));
        }

        try
        {
            await _requestChannel.Writer.WriteAsync(request, cancellationToken).ConfigureAwait(false);
            return await taskCompletionSource.Task.ConfigureAwait(false);
        }
        finally
        {
            registration?.Dispose();
        }
    }

    public bool PublishResponse(ResponseMessage response)
    {
        if (_pendingRequests.TryRemove(response.CorrelationId, out var pendingRequest))
        {
            pendingRequest.TrySetResult(response);
            return true;
        }

        return false;
    }

    public void StopAcceptingRequests()
    {
        _requestChannel.Writer.TryComplete();
    }

    public async ValueTask DisposeAsync()
    {
        StopAcceptingRequests();

        foreach (var pair in _pendingRequests)
        {
            if (_pendingRequests.TryRemove(pair.Key, out var pending))
            {
                pending.TrySetCanceled();
            }
        }

        if (_requestChannel.Reader.Completion is { IsCompleted: false })
        {
            try
            {
                await _requestChannel.Reader.Completion.ConfigureAwait(false);
            }
            catch (ChannelClosedException)
            {
                // ignored
            }
        }
    }
}
