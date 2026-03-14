using Aida.ParallelChange.Api.Infrastructure.Http.Errors;
using Aida.ParallelChange.Api.Infrastructure.Http.Mappers.V1;

namespace Aida.ParallelChange.Api.Tests.Unit.Infrastructure.Http.Mappers.V1;

[TestFixture]
public sealed class CustomerIdRouteParserTests
{
    [Test]
    public void Parse_returns_customer_id_when_route_value_is_valid()
    {
        var customerId = CustomerIdRouteParser.Parse("42");

        customerId.Value.ShouldBe(42);
    }

    [Test]
    public void Parse_throws_validation_error_when_route_value_is_not_numeric()
    {
        var exception = Should.Throw<ApiRequestValidationException>(() => CustomerIdRouteParser.Parse("invalid"));

        exception.Title.ShouldBe("Invalid customer id");
        exception.Message.ShouldBe("Customer id must be a number greater than zero.");
    }

    [Test]
    public void Parse_throws_validation_error_when_route_value_is_zero()
    {
        var exception = Should.Throw<ApiRequestValidationException>(() => CustomerIdRouteParser.Parse("0"));

        exception.Title.ShouldBe("Invalid customer id");
        exception.Message.ShouldStartWith("Customer id must be greater than zero.");
    }
}
