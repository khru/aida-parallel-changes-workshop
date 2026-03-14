namespace Aida.ParallelChange.Api.Domain;

public sealed class PhoneNumber
{
    private const int MaximumLength = 30;

    private const string ValueIsRequiredMessage = "Phone number is required.";
    private const string ValueIsInvalidMessage = "Phone number is invalid.";

    public string Value { get; }

    public PhoneNumber(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException(ValueIsRequiredMessage, nameof(value));
        }

        var trimmedValue = value.Trim();

        if (trimmedValue.Length > MaximumLength)
        {
            throw new ArgumentException(ValueIsInvalidMessage, nameof(value));
        }

        Value = trimmedValue;
    }
}
