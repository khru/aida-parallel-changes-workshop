using Aida.ParallelChange.Api.Infrastructure.InMemory;
using Microsoft.AspNetCore.Mvc;

namespace Aida.ParallelChange.Api.Controllers.V1;

[ApiController]
[Route("api/v1/customer-contacts")]
public sealed class CustomerContactsV1Controller : ControllerBase
{
    private readonly LegacyCustomerContactStore _store;

    public CustomerContactsV1Controller(LegacyCustomerContactStore store)
    {
        _store = store;
    }

    [HttpGet("{customerId:int}")]
    [Produces("application/vnd.api+json")]
    public IActionResult Get(int customerId)
    {
        var record = _store.FindById(customerId);

        if (record is null)
        {
            return NotFound();
        }

        var document = new
        {
            data = new
            {
                type = "customer-contacts",
                id = record.CustomerId.ToString(),
                attributes = new
                {
                    contactName = record.ContactName,
                    phone = record.Phone,
                    email = record.Email
                }
            }
        };

        return new JsonResult(document)
        {
            ContentType = "application/vnd.api+json"
        };
    }
}
