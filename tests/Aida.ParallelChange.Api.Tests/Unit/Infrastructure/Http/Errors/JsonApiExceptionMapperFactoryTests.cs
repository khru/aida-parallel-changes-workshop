using Aida.ParallelChange.Api.Application.CreateCustomerContact;
using Aida.ParallelChange.Api.Application.UpdateCustomerContact;
using Aida.ParallelChange.Api.Infrastructure.Http.Errors;
using Microsoft.AspNetCore.Http;

namespace Aida.ParallelChange.Api.Tests.Unit.Infrastructure.Http.Errors;

[TestFixture]
public sealed class JsonApiExceptionMapperFactoryTests
{
    [Test]
    public void Create_returns_bad_request_response_for_validation_exception()
    {
        var factory = CreateFactory();
        var exception = new ApiRequestValidationException("Invalid customer id", "Customer id must be a number greater than zero.");

        var response = factory.Create(exception);

        response.StatusCode.ShouldBe(400);
        response.Title.ShouldBe("Invalid customer id");
        response.Detail.ShouldBe("Customer id must be a number greater than zero.");
    }

    [Test]
    public void Create_returns_conflict_response_for_already_exists_exception()
    {
        var factory = CreateFactory();
        var exception = new CustomerContactAlreadyExistsException(42);

        var response = factory.Create(exception);

        response.StatusCode.ShouldBe(409);
        response.Title.ShouldBe("Customer contact already exists");
        response.Detail.ShouldBe("Customer contact '42' already exists.");
    }

    [Test]
    public void Create_returns_not_found_response_for_not_found_exception()
    {
        var factory = CreateFactory();
        var exception = new CustomerContactNotFoundException(99);

        var response = factory.Create(exception);

        response.StatusCode.ShouldBe(404);
        response.Title.ShouldBe("Customer contact not found");
        response.Detail.ShouldBe("Customer contact '99' was not found.");
    }

    [Test]
    public void Create_returns_bad_request_response_for_bad_http_request_exception()
    {
        var factory = CreateFactory();
        var exception = new BadHttpRequestException("Malformed");

        var response = factory.Create(exception);

        response.StatusCode.ShouldBe(400);
        response.Title.ShouldBe("Invalid request");
        response.Detail.ShouldBe("Invalid request.");
    }

    [Test]
    public void Create_returns_unexpected_response_for_unknown_exception()
    {
        var factory = CreateFactory();
        var exception = new InvalidOperationException("Boom");

        var response = factory.Create(exception);

        response.StatusCode.ShouldBe(500);
        response.Title.ShouldBe("Unexpected error");
        response.Detail.ShouldBe("An unexpected error occurred while processing the request.");
    }

    private static JsonApiExceptionMapperFactory CreateFactory()
    {
        return new JsonApiExceptionMapperFactory(
        [
            new ApiRequestValidationJsonApiExceptionMapper(),
            new CustomerContactAlreadyExistsJsonApiExceptionMapper(),
            new CustomerContactNotFoundJsonApiExceptionMapper(),
            new BadHttpRequestJsonApiExceptionMapper(),
            new UnexpectedJsonApiExceptionMapper()
        ]);
    }
}
