using AnticorruptionLayer.Acl;
using AnticorruptionLayer.Domain;
using AnticorruptionLayer.Legacy;

namespace AnticorruptionLayer;

public static class Program
{
	public static void Main()
	{
		Console.WriteLine("=== Anti-Corruption Layer demo ===\n");

		var legacyService = new LegacyCrmService();
		var gateway = new LegacyCustomerAdapter(legacyService);
		var onboarding = new CustomerOnboardingService(gateway);

		foreach (var customerId in new[] { "CUST-1001", "CUST-1002", "CUST-9999" })
		{
			try
			{
				var profile = onboarding.GetCustomerProfile(customerId);
				Console.WriteLine(
					$"Cliente {profile.Id} ({profile.FullName}) => Segmento: {profile.Category} | Preferido: {(profile.IsPreferred ? "Sí" : "No")}");
			}
			catch (InvalidOperationException ex)
			{
				Console.WriteLine($"[ADVERTENCIA] {ex.Message}");
			}
		}
	}
}
