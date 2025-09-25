using AnticorruptionLayer.Legacy.Models;

namespace AnticorruptionLayer.Legacy;

/// <summary>
/// Simulates a legacy system with a proprietary API and peculiar data format.
/// </summary>
public class LegacyCrmService
{
    private static readonly Dictionary<string, LegacyCustomerRecord> Database = new()
    {
        ["CUST-1001"] = new LegacyCustomerRecord("CUST-1001", "Ana Gómez", 3, 62000m),
        ["CUST-1002"] = new LegacyCustomerRecord("CUST-1002", "Luis Ramírez", 1, 4800m),
        ["CUST-1003"] = new LegacyCustomerRecord("CUST-1003", "María Díaz", 2, 18000m)
    };

    public LegacyCustomerRecord? GetCustomer(string externalId)
    {
        // Imagine some SOAP/XML or mainframe call happening here.
        Database.TryGetValue(externalId, out var record);
        return record;
    }
}
