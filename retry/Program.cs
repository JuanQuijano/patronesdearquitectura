using RetryPattern.Resilience;
using RetryPattern.Services;

Console.WriteLine("=== Circuit Breaker + Retry demo ===\n");

var service = new UnreliableExternalService(unstableDuration: TimeSpan.FromSeconds(6));

var breaker = new CircuitBreaker(
	new CircuitBreakerOptions
	{
		FailureThreshold = 3,
		OpenToHalfOpenWait = TimeSpan.FromSeconds(3),
		HalfOpenMaxAttempts = 4
	},
	new RetryPolicy(
		maxAttempts: 4,
		delayStrategy: attempt => TimeSpan.FromMilliseconds(400 * attempt),
		onRetry: (attempt, ex) => Console.WriteLine($"    ↩️ Reintento {attempt} en semi-abierto: {ex.Message}")));

for (var iteration = 1; iteration <= 14; iteration++)
{
	Console.WriteLine($"Intento #{iteration} | Estado: {breaker.CurrentState}");

	try
	{
		var payload = breaker.Execute(() => service.GetResource());
		Console.WriteLine($"  ✅ Respuesta: {payload}");
	}
	catch (CircuitOpenException ex)
	{
		Console.WriteLine($"  🔌 Circuito abierto. Reintenta en ~{ex.RetryAfter.TotalSeconds:F1}s");
	}
	catch (Exception ex)
	{
		Console.WriteLine($"  ❌ Error: {ex.Message}");
	}

	Thread.Sleep(700);
}

Console.WriteLine("\nDemostración finalizada.");
