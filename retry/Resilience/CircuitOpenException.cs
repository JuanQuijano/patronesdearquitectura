namespace RetryPattern.Resilience;

public class CircuitOpenException : Exception
{
    public TimeSpan RetryAfter { get; }

    public CircuitOpenException(TimeSpan retryAfter)
        : base($"Circuit breaker is open. Retry after {retryAfter.TotalMilliseconds:F0} ms.")
    {
        RetryAfter = retryAfter;
    }
}
