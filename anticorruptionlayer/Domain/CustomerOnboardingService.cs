namespace AnticorruptionLayer.Domain;

/// <summary>
/// Application service that depends on the Anti-Corruption Layer instead of the legacy API.
/// </summary>
public class CustomerOnboardingService(ICustomerGateway customerGateway)
{
    private readonly ICustomerGateway _customerGateway = customerGateway;

    public CustomerProfile GetCustomerProfile(string externalId)
    {
        var snapshot = _customerGateway.FetchCustomer(externalId);

        return snapshot switch
        {
            null => throw new InvalidOperationException(
                $"The external system did not return data for customer '{externalId}'."),
            _ => new CustomerProfile(
                snapshot.ExternalId,
                snapshot.DisplayName,
                snapshot.Segment,
                snapshot.Score >= 80)
        };
    }
}
