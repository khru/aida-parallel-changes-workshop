namespace Aida.ParallelChange.Api.Domain;

public sealed class EmailAddress
{
    public string Value { get; }

    public EmailAddress(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || !value.Contains('@'))
        {
            throw new ArgumentException("Email address is invalid.", nameof(value));
        }

        Value = value.Trim();
    }
}
