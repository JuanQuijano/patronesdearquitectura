using asynchronousRequestReply.Messaging;

namespace asynchronousRequestReply.Services;

public sealed class UppercaseResponder
{
    private readonly AsyncRequestReplyBus _bus;
    private readonly Random _random = new();

    public UppercaseResponder(AsyncRequestReplyBus bus)
    {
        _bus = bus;
    }

    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        await foreach (var request in _bus.Requests.ReadAllAsync(cancellationToken).ConfigureAwait(false))
        {
            try
            {
                // Simulate unpredictable processing time.
                var delay = _random.Next(200, 800);
                await Task.Delay(delay, cancellationToken).ConfigureAwait(false);

                var processedPayload = request.Payload.ToUpperInvariant();
                var responseText = $"[{delay}ms] Respuesta procesada: {processedPayload}";

                _bus.PublishResponse(ResponseMessage.Success(request.CorrelationId, responseText));
            }
            catch (OperationCanceledException)
            {
                _bus.PublishResponse(ResponseMessage.Failure(request.CorrelationId, "Solicitud cancelada."));
            }
            catch (Exception ex)
            {
                _bus.PublishResponse(ResponseMessage.Failure(request.CorrelationId, ex.Message));
            }
        }
    }
}
