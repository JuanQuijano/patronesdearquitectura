namespace AnticorruptionLayer.Domain;

/// <summary>
/// Hexagonal port used by the domain to interact with foreign systems.
/// </summary>
public interface ICustomerGateway
{
    CustomerSnapshot? FetchCustomer(string externalId);
}
