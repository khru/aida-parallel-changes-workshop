using Aida.ParallelChange.Api.Application;

namespace Aida.ParallelChange.Api.Application.CreateCustomerContact;

public sealed class CustomerContactAlreadyExistsException : Exception
{
    public int CustomerId { get; }

    public CustomerContactAlreadyExistsException(int customerId)
        : base(CustomerContactErrorMessages.BuildAlreadyExistsDetail(customerId))
    {
        CustomerId = customerId;
    }
}
