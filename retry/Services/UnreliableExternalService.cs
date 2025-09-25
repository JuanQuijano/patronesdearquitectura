namespace RetryPattern.Services;

/// <summary>
/// Simulates a remote dependency that remains unhealthy for an initial period of time.
/// </summary>
public class UnreliableExternalService
{
    private readonly DateTime _unstableUntilUtc;
    private readonly Random _random = new();

    public UnreliableExternalService(TimeSpan unstableDuration)
    {
        _unstableUntilUtc = DateTime.UtcNow + unstableDuration;
    }

    public string GetResource()
    {
        if (DateTime.UtcNow < _unstableUntilUtc)
        {
            throw new TimeoutException("Downstream service timed out.");
        }

        // Still include a bit of randomness once recovered to mimic a real service.
        var temperature = 20 + _random.Next(-3, 4);
        return $"External payload retrieved at {DateTime.Now:T} (temp: {temperature}°C)";
    }
}
