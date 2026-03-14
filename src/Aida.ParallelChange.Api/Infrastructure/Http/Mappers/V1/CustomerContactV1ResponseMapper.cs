using Aida.ParallelChange.Api.Domain;
using Aida.ParallelChange.Api.Infrastructure.Http.Contracts.JsonApi;
using Aida.ParallelChange.Api.Infrastructure.Http.Contracts.V1;

namespace Aida.ParallelChange.Api.Infrastructure.Http.Mappers.V1;

public static class CustomerContactV1ResponseMapper
{
    private const string ResourceType = "customer-contacts";

    public static CustomerContactV1Document ToDocument(CustomerContact customerContact)
    {
        return new CustomerContactV1Document
        {
            Data = new JsonApiResource<CustomerContactV1Attributes>
            {
                Type = ResourceType,
                Id = customerContact.CustomerId.Value.ToString(),
                Attributes = new CustomerContactV1Attributes
                {
                    ContactName = customerContact.ContactName.Value,
                    Phone = customerContact.Phone.Value,
                    Email = customerContact.Email.Value
                }
            }
        };
    }
}
