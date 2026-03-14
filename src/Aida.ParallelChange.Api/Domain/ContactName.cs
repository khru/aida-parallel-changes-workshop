namespace Aida.ParallelChange.Api.Domain;

public sealed record ContactName
{
    private const string NameIsRequiredMessage = "Contact name is required.";

    public string Value { get; }

    public ContactName(string contactName)
    {
        var normalizedContactName = contactName?.Trim();

        if (string.IsNullOrWhiteSpace(normalizedContactName))
        {
            throw new ArgumentException(NameIsRequiredMessage, nameof(contactName));
        }

        Value = normalizedContactName;
    }
}
