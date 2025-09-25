namespace RetryPattern.Resilience;

public enum CircuitBreakerState
{
    Closed,
    Open,
    HalfOpen
}
