namespace Aida.ParallelChange.Api.Domain;

public sealed class ContactName
{
    private const string NameIsRequiredMessage = "Contact name is required.";

    public string Value { get; }

    public ContactName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException(NameIsRequiredMessage, nameof(value));
        }

        Value = value.Trim();
    }
}
