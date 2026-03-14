namespace Aida.ParallelChange.Api.Domain;

public sealed class PhoneNumber
{
    public string Value { get; }

    public PhoneNumber(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException(CustomerContactDomainRules.PhoneNumberIsRequiredMessage, nameof(value));
        }

        var trimmedValue = value.Trim();

        if (trimmedValue.Length > CustomerContactDomainRules.MaximumPhoneNumberLength)
        {
            throw new ArgumentException(CustomerContactDomainRules.PhoneNumberIsInvalidMessage, nameof(value));
        }

        Value = trimmedValue;
    }
}
