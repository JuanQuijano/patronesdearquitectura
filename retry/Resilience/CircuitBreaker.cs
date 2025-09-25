namespace RetryPattern.Resilience;

/// <summary>
/// Simplified circuit breaker supporting Closed, Open and Half-Open states.
/// In Half-Open it leverages a retry policy to probe the downstream system.
/// </summary>
public class CircuitBreaker
{
    private readonly object _syncRoot = new();
    private readonly CircuitBreakerOptions _options;
    private readonly RetryPolicy _halfOpenRetryPolicy;

    private CircuitBreakerState _state = CircuitBreakerState.Closed;
    private int _failureCount;
    private DateTime _openSinceUtc;

    public CircuitBreaker(CircuitBreakerOptions options, RetryPolicy? halfOpenRetryPolicy = null)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));

        if (_options.FailureThreshold <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(options.FailureThreshold));
        }

        if (_options.HalfOpenMaxAttempts <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(options.HalfOpenMaxAttempts));
        }

        _halfOpenRetryPolicy = halfOpenRetryPolicy ?? new RetryPolicy(_options.HalfOpenMaxAttempts);
    }

    public CircuitBreakerState CurrentState => GetState(out _);

    public T Execute<T>(Func<T> action)
    {
        var state = GetState(out var remainingOpenTime);

        switch (state)
        {
            case CircuitBreakerState.Open:
                throw new CircuitOpenException(remainingOpenTime ?? _options.OpenToHalfOpenWait);
            case CircuitBreakerState.HalfOpen:
                return ProbeHalfOpen(action);
            case CircuitBreakerState.Closed:
            default:
                return ExecuteClosed(action);
        }
    }

    private T ExecuteClosed<T>(Func<T> action)
    {
        try
        {
            var result = action();
            Reset();
            return result;
        }
        catch (Exception ex)
        {
            RegisterFailure(ex);
            throw;
        }
    }

    private T ProbeHalfOpen<T>(Func<T> action)
    {
        try
        {
            var result = _halfOpenRetryPolicy.Execute(action);
            TransitionToClosed();
            return result;
        }
        catch (Exception)
        {
            Trip();
            throw;
        }
    }

    private void RegisterFailure(Exception exception)
    {
        _ = exception;
        lock (_syncRoot)
        {
            _failureCount++;
            if (_failureCount >= _options.FailureThreshold)
            {
                Trip();
            }
        }
    }

    private void Trip()
    {
        lock (_syncRoot)
        {
            _state = CircuitBreakerState.Open;
            _openSinceUtc = DateTime.UtcNow;
            _failureCount = 0;
        }
    }

    private void TransitionToClosed()
    {
        lock (_syncRoot)
        {
            _state = CircuitBreakerState.Closed;
            _failureCount = 0;
        }
    }

    private void Reset()
    {
        lock (_syncRoot)
        {
            _failureCount = 0;
        }
    }

    private CircuitBreakerState GetState(out TimeSpan? remainingOpenTime)
    {
        lock (_syncRoot)
        {
            if (_state == CircuitBreakerState.Open)
            {
                var elapsed = DateTime.UtcNow - _openSinceUtc;
                if (elapsed >= _options.OpenToHalfOpenWait)
                {
                    _state = CircuitBreakerState.HalfOpen;
                    remainingOpenTime = null;
                    return _state;
                }

                remainingOpenTime = _options.OpenToHalfOpenWait - elapsed;
                return _state;
            }

            remainingOpenTime = null;
            return _state;
        }
    }
}
