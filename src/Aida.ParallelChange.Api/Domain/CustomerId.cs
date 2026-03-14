namespace Aida.ParallelChange.Api.Domain;

public readonly record struct CustomerId
{
    public int Value { get; }

    public CustomerId(int value)
    {
        if (value < CustomerContactDomainRules.MinimumCustomerIdValue)
        {
            throw new ArgumentOutOfRangeException(nameof(value), CustomerContactDomainRules.CustomerIdMustBeGreaterThanZeroMessage);
        }

        Value = value;
    }
}
