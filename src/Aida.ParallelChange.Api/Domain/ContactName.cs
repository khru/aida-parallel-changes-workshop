namespace Aida.ParallelChange.Api.Domain;

public sealed class ContactName
{
    public string Value { get; }

    public ContactName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException(CustomerContactDomainRules.ContactNameIsRequiredMessage, nameof(value));
        }

        Value = value.Trim();
    }
}
