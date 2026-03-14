using Aida.ParallelChange.Api.Application.CreateCustomerContact;
using Aida.ParallelChange.Api.Application.GetCustomerContact;
using Aida.ParallelChange.Api.Application.UpdateCustomerContact;
using Aida.ParallelChange.Api.Domain;
using Aida.ParallelChange.Api.Infrastructure.Http.Contracts.JsonApi;
using Aida.ParallelChange.Api.Infrastructure.Http.Errors;
using Aida.ParallelChange.Api.Infrastructure.Http.Contracts.V1;
using Aida.ParallelChange.Api.Infrastructure.Http.JsonApi;
using Microsoft.AspNetCore.Mvc;

namespace Aida.ParallelChange.Api.Infrastructure.Http.Controllers.V1;

[ApiController]
[Route("api/v1/customer-contacts")]
public sealed class CustomerContactsV1Controller : ControllerBase
{
    private const string ResourceType = "customer-contacts";
    private const string CustomerContactsLocationFormat = "/api/v1/customer-contacts/{0}";

    private readonly GetCustomerContactHandler _getHandler;
    private readonly CreateCustomerContactHandler _createHandler;
    private readonly UpdateCustomerContactHandler _updateHandler;

    public CustomerContactsV1Controller(
        GetCustomerContactHandler getHandler,
        CreateCustomerContactHandler createHandler,
        UpdateCustomerContactHandler updateHandler)
    {
        _getHandler = getHandler;
        _createHandler = createHandler;
        _updateHandler = updateHandler;
    }

    [HttpGet("{customerId}")]
    [Produces(JsonApiMediaTypes.JsonApi)]
    [ProducesResponseType(typeof(CustomerContactV1Document), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(string customerId, CancellationToken cancellationToken)
    {
        var parsedCustomerId = CreateCustomerId(customerId);
        var result = await _getHandler.HandleAsync(new GetCustomerContactQuery(parsedCustomerId), cancellationToken);

        if (result is FindCustomerContactResult.NotFound notFound)
        {
            throw new CustomerContactNotFoundException(notFound.CustomerId.Value);
        }

        var found = (FindCustomerContactResult.Found)result;
        var document = ToDocument(found.CustomerContact);

        Response.ContentType = JsonApiMediaTypes.JsonApi;
        return Ok(document);
    }

    [HttpPost]
    [Consumes(JsonApiMediaTypes.JsonApi)]
    [Produces(JsonApiMediaTypes.JsonApi)]
    [ProducesResponseType(typeof(CustomerContactV1Document), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Post([FromBody] CreateCustomerContactV1Request request, CancellationToken cancellationToken)
    {
        var customerContact = CreateCustomerContact(request.CustomerId, request.ContactName, request.Phone, request.Email);

        await _createHandler.HandleAsync(new CreateCustomerContactCommand(customerContact), cancellationToken);

        var location = string.Format(CustomerContactsLocationFormat, customerContact.CustomerId.Value);
        Response.ContentType = JsonApiMediaTypes.JsonApi;
        return Created(location, ToDocument(customerContact));
    }

    [HttpPut("{customerId}")]
    [Consumes(JsonApiMediaTypes.JsonApi)]
    [Produces(JsonApiMediaTypes.JsonApi)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Put(string customerId, [FromBody] UpdateCustomerContactV1Request request, CancellationToken cancellationToken)
    {
        var parsedCustomerId = CreateCustomerId(customerId);
        var customerContact = CreateCustomerContact(parsedCustomerId.Value, request.ContactName, request.Phone, request.Email);

        await _updateHandler.HandleAsync(new UpdateCustomerContactCommand(customerContact), cancellationToken);

        return NoContent();
    }

    private static CustomerId CreateCustomerId(string rawCustomerId)
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

    private static CustomerContact CreateCustomerContact(
        int customerId,
        string contactName,
        string phone,
        string email)
    {
        try
        {
            return new CustomerContact(
                new CustomerId(customerId),
                new ContactName(contactName),
                new PhoneNumber(phone),
                new EmailAddress(email));
        }
        catch (Exception exception) when (exception is ArgumentException or ArgumentOutOfRangeException)
        {
            throw new ApiRequestValidationException(JsonApiErrorCatalog.InvalidRequestTitle, exception.Message);
        }
    }

    private static CustomerContactV1Document ToDocument(CustomerContact contact)
    {
        return new CustomerContactV1Document
        {
            Data = new JsonApiResource<CustomerContactV1Attributes>
            {
                Type = ResourceType,
                Id = contact.CustomerId.Value.ToString(),
                Attributes = new CustomerContactV1Attributes
                {
                    ContactName = contact.ContactName.Value,
                    Phone = contact.Phone.Value,
                    Email = contact.Email.Value
                }
            }
        };
    }
}
