using Aida.ParallelChange.Api.Domain;

namespace Aida.ParallelChange.Api.Application.GetCustomerContact;

public abstract record FindCustomerContactResult
{
    public sealed record Found(CustomerContact CustomerContact) : FindCustomerContactResult;

    public sealed record NotFound(CustomerId CustomerId) : FindCustomerContactResult;
}
