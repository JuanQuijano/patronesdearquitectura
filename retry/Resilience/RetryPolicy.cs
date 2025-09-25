namespace RetryPattern.Resilience;

/// <summary>
/// Simple retry policy with configurable attempts and delay strategy.
/// </summary>
public class RetryPolicy
{
    private readonly int _maxAttempts;
    private readonly Func<int, TimeSpan> _delayStrategy;
    private readonly Action<int, Exception>? _onRetry;

    public RetryPolicy(int maxAttempts, Func<int, TimeSpan>? delayStrategy = null, Action<int, Exception>? onRetry = null)
    {
        if (maxAttempts <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(maxAttempts), "Max attempts must be greater than zero.");
        }

        _maxAttempts = maxAttempts;
        _delayStrategy = delayStrategy ?? (_ => TimeSpan.Zero);
        _onRetry = onRetry;
    }

    public T Execute<T>(Func<T> action)
    {
        if (action is null)
        {
            throw new ArgumentNullException(nameof(action));
        }

        Exception? lastException = null;

        for (var attempt = 1; attempt <= _maxAttempts; attempt++)
        {
            try
            {
                return action();
            }
            catch (Exception ex)
            {
                lastException = ex;

                if (attempt >= _maxAttempts)
                {
                    throw;
                }

                _onRetry?.Invoke(attempt, ex);
                Thread.Sleep(_delayStrategy(attempt));
            }
        }

        throw lastException ?? new InvalidOperationException("RetryPolicy ended without executing action.");
    }

    public async Task<T> ExecuteAsync<T>(Func<Task<T>> action)
    {
        if (action is null)
        {
            throw new ArgumentNullException(nameof(action));
        }

        Exception? lastException = null;

        for (var attempt = 1; attempt <= _maxAttempts; attempt++)
        {
            try
            {
                return await action();
            }
            catch (Exception ex)
            {
                lastException = ex;

                if (attempt >= _maxAttempts)
                {
                    throw;
                }

                _onRetry?.Invoke(attempt, ex);
                await Task.Delay(_delayStrategy(attempt));
            }
        }

        throw lastException ?? new InvalidOperationException("RetryPolicy ended without executing action.");
    }
}
