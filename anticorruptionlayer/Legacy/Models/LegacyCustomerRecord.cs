namespace AnticorruptionLayer.Legacy.Models;

/// <summary>
/// Representation of the data returned by the legacy CRM.
/// </summary>
public record LegacyCustomerRecord(string ExternalId, string FullName, int Level, decimal LifetimeValue);
