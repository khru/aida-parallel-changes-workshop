namespace Aida.ParallelChange.Api.Domain;

public sealed class EmailAddress
{
    public string Value { get; }

    public EmailAddress(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException(CustomerContactDomainRules.EmailAddressIsInvalidMessage, nameof(value));
        }

        var trimmedValue = value.Trim();

        if (!trimmedValue.Contains(CustomerContactDomainRules.EmailAddressAtSymbol))
        {
            throw new ArgumentException(CustomerContactDomainRules.EmailAddressIsInvalidMessage, nameof(value));
        }

        Value = trimmedValue;
    }
}
