namespace Aida.ParallelChange.Api.Domain;

public sealed record PhoneNumber
{
    private const int MaximumLength = 30;

    private const string ValueIsRequiredMessage = "Phone number is required.";
    private const string ValueIsInvalidMessage = "Phone number is invalid.";

    public string Value { get; }

    public PhoneNumber(string phoneNumber)
    {
        var normalizedPhoneNumber = phoneNumber?.Trim();

        if (string.IsNullOrWhiteSpace(normalizedPhoneNumber))
        {
            throw new ArgumentException(ValueIsRequiredMessage, nameof(phoneNumber));
        }

        if (normalizedPhoneNumber.Length > MaximumLength)
        {
            throw new ArgumentException(ValueIsInvalidMessage, nameof(phoneNumber));
        }

        Value = normalizedPhoneNumber;
    }
}
