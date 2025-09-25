namespace RetryPattern.Resilience;

public class CircuitBreakerOptions
{
    public int FailureThreshold { get; init; } = 3;
    public TimeSpan OpenToHalfOpenWait { get; init; } = TimeSpan.FromSeconds(30);
    public int HalfOpenMaxAttempts { get; init; } = 1;
}
