namespace Aida.ParallelChange.Api.Domain;

public sealed class EmailAddress
{
    private const char AtSymbol = '@';
    private const string ValueIsInvalidMessage = "Email address is invalid.";

    public string Value { get; }

    public EmailAddress(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException(ValueIsInvalidMessage, nameof(value));
        }

        var trimmedValue = value.Trim();

        if (!trimmedValue.Contains(AtSymbol))
        {
            throw new ArgumentException(ValueIsInvalidMessage, nameof(value));
        }

        Value = trimmedValue;
    }
}
