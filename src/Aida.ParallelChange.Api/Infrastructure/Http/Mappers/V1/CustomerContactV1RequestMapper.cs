using Aida.ParallelChange.Api.Domain;
using Aida.ParallelChange.Api.Infrastructure.Http.Errors;

namespace Aida.ParallelChange.Api.Infrastructure.Http.Mappers.V1;

public static class CustomerContactV1RequestMapper
{
    private const string InvalidRequestTitle = "Invalid request";

    public static CustomerContact ToDomain(int customerId, string contactName, string phone, string email)
    {
        try
        {
            return CustomerContactBuilder.Create()
                .WithCustomerId(customerId)
                .WithContactName(contactName)
                .WithPhoneNumber(phone)
                .WithEmailAddress(email)
                .Build();
        }
        catch (Exception exception) when (exception is ArgumentException or ArgumentOutOfRangeException)
        {
            throw new ApiRequestValidationException(InvalidRequestTitle, exception.Message);
        }
    }
}
