using Aida.ParallelChange.Api.Domain;
using Aida.ParallelChange.Api.Infrastructure.Http.Errors;

namespace Aida.ParallelChange.Api.Infrastructure.Http.Mappers.V1;

public static class CustomerIdRouteParser
{
    private const string InvalidCustomerIdTitle = "Invalid customer id";
    private const string InvalidCustomerIdNumberDetail = "Customer id must be a number greater than zero.";

    public static CustomerId Parse(string rawCustomerId)
    {
        if (!int.TryParse(rawCustomerId, out var customerIdValue))
        {
            throw new ApiRequestValidationException(
                InvalidCustomerIdTitle,
                InvalidCustomerIdNumberDetail);
        }

        try
        {
            return new CustomerId(customerIdValue);
        }
        catch (ArgumentOutOfRangeException exception)
        {
            throw new ApiRequestValidationException(InvalidCustomerIdTitle, exception.Message);
        }
    }
}
