namespace Aida.ParallelChange.Api.Domain;

public static class CustomerContactDomainRules
{
    public const int MinimumCustomerIdValue = 1;
    public const int MaximumPhoneNumberLength = 30;
    public const char EmailAddressAtSymbol = '@';

    public const string CustomerIdMustBeGreaterThanZeroMessage = "Customer id must be greater than zero.";
    public const string ContactNameIsRequiredMessage = "Contact name is required.";
    public const string PhoneNumberIsRequiredMessage = "Phone number is required.";
    public const string PhoneNumberIsInvalidMessage = "Phone number is invalid.";
    public const string EmailAddressIsInvalidMessage = "Email address is invalid.";
}
