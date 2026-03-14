namespace Aida.ParallelChange.Api.Domain;

public sealed record EmailAddress
{
    private const char AtSymbol = '@';
    private const string ValueIsInvalidMessage = "Email address is invalid.";

    public string Value { get; }

    public EmailAddress(string emailAddress)
    {
        var normalizedEmailAddress = emailAddress?.Trim();

        if (string.IsNullOrWhiteSpace(normalizedEmailAddress))
        {
            throw new ArgumentException(ValueIsInvalidMessage, nameof(emailAddress));
        }

        if (!normalizedEmailAddress.Contains(AtSymbol))
        {
            throw new ArgumentException(ValueIsInvalidMessage, nameof(emailAddress));
        }

        Value = normalizedEmailAddress;
    }
}
