using Aida.ParallelChange.Api.Domain;
using Aida.ParallelChange.Api.Infrastructure.Http.Errors;

namespace Aida.ParallelChange.Api.Infrastructure.Http.Mappers.V1;

public static class CustomerIdRouteParser
{
    public static CustomerId Parse(string rawCustomerId)
    {
        if (!int.TryParse(rawCustomerId, out var customerIdValue))
        {
            throw new ApiRequestValidationException(
                JsonApiErrorCatalog.InvalidCustomerIdTitle,
                JsonApiErrorCatalog.InvalidCustomerIdNumberDetail);
        }

        try
        {
            return new CustomerId(customerIdValue);
        }
        catch (ArgumentOutOfRangeException exception)
        {
            throw new ApiRequestValidationException(JsonApiErrorCatalog.InvalidCustomerIdTitle, exception.Message);
        }
    }
}
