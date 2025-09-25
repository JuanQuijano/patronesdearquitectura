using AnticorruptionLayer.Domain;
using AnticorruptionLayer.Legacy;
using AnticorruptionLayer.Legacy.Models;

namespace AnticorruptionLayer.Acl;

/// <summary>
/// Translates the legacy CRM contract into a domain-friendly data structure.
/// </summary>
public class LegacyCustomerAdapter(LegacyCrmService legacyService) : ICustomerGateway
{
    private readonly LegacyCrmService _legacyService = legacyService;

    public CustomerSnapshot? FetchCustomer(string externalId)
    {
        LegacyCustomerRecord? record = _legacyService.GetCustomer(externalId);
        if (record is null)
        {
            return null;
        }

        return new CustomerSnapshot(
            record.ExternalId,
            record.FullName,
            record.Level switch
            {
                1 => "Standard",
                2 => "Gold",
                3 => "Platinum",
                _ => "Unknown"
            },
            MapScore(record.LifetimeValue));
    }

    private static int MapScore(decimal lifetimeValue) => lifetimeValue switch
    {
        >= 50000m => 95,
        >= 20000m => 85,
        >= 5000m => 70,
        _ => 40
    };
}
