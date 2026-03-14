using Aida.ParallelChange.Api.Infrastructure.Http.Errors;
using Aida.ParallelChange.Api.Infrastructure.Http.Mappers.V1;

namespace Aida.ParallelChange.Api.Tests.Unit.Infrastructure.Http.Mappers.V1;

[TestFixture]
public sealed class CustomerContactV1RequestMapperTests
{
    [Test]
    public void ToDomain_returns_customer_contact_when_payload_is_valid()
    {
        var contact = CustomerContactV1RequestMapper.ToDomain(7, "Grace Hopper", "+1 5550100", "grace.hopper@example.com");

        contact.CustomerId.Value.ShouldBe(7);
        contact.ContactName.Value.ShouldBe("Grace Hopper");
    }

    [Test]
    public void ToDomain_throws_validation_error_when_payload_is_invalid()
    {
        var exception = Should.Throw<ApiRequestValidationException>(() =>
            CustomerContactV1RequestMapper.ToDomain(7, string.Empty, "+1 5550100", "grace.hopper@example.com"));

        exception.Title.ShouldBe("Invalid request");
        exception.Message.ShouldStartWith("Contact name is required.");
    }
}
