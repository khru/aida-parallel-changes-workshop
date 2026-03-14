using Aida.ParallelChange.Api.Application;

namespace Aida.ParallelChange.Api.Application.UpdateCustomerContact;

public sealed class CustomerContactNotFoundException : Exception
{
    public int CustomerId { get; }

    public CustomerContactNotFoundException(int customerId)
        : base(CustomerContactErrorMessages.BuildNotFoundDetail(customerId))
    {
        CustomerId = customerId;
    }
}
