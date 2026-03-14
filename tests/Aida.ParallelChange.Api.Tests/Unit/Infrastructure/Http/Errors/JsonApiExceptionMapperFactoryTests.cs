using Aida.ParallelChange.Api.Application.CreateCustomerContact;
using Aida.ParallelChange.Api.Application.UpdateCustomerContact;
using Aida.ParallelChange.Api.Infrastructure.Http.Errors;
using Microsoft.AspNetCore.Http;

namespace Aida.ParallelChange.Api.Tests.Unit.Infrastructure.Http.Errors;

[TestFixture]
public sealed class JsonApiExceptionMapperFactoryTests
{
    private JsonApiExceptionMapperFactory _factory = default!;

    [SetUp]
    public void SetUp()
    {
        _factory = new JsonApiExceptionMapperFactory(
        [
            new ApiRequestValidationJsonApiExceptionMapper(),
            new CustomerContactAlreadyExistsJsonApiExceptionMapper(),
            new CustomerContactNotFoundJsonApiExceptionMapper(),
            new BadHttpRequestJsonApiExceptionMapper(),
            new UnexpectedJsonApiExceptionMapper()
        ]);
    }

    [TestCaseSource(nameof(ExceptionCases))]
    public void Create_returns_expected_response_for_known_exception(
        Func<Exception> exceptionFactory,
        int expectedStatusCode,
        string expectedTitle,
        string expectedDetail,
        Action<Exception> assertExceptionState)
    {
        var exception = exceptionFactory();
        assertExceptionState(exception);

        var response = _factory.Create(exception);

        response.StatusCode.ShouldBe(expectedStatusCode);
        response.Title.ShouldBe(expectedTitle);
        response.Detail.ShouldBe(expectedDetail);
    }

    private static IEnumerable<TestCaseData> ExceptionCases()
    {
        return
        [
            new TestCaseData(
                () => (Exception)new ApiRequestValidationException("Invalid customer id", "Customer id must be a number greater than zero."),
                StatusCodes.Status400BadRequest,
                "Invalid customer id",
                "Customer id must be a number greater than zero.",
                (Action<Exception>)(_ => { })),
            new TestCaseData(
                () => (Exception)new CustomerContactAlreadyExistsException(42),
                StatusCodes.Status409Conflict,
                "Customer contact already exists",
                "Customer contact '42' already exists.",
                (Action<Exception>)(exception => ((CustomerContactAlreadyExistsException)exception).CustomerId.ShouldBe(42))),
            new TestCaseData(
                () => (Exception)new CustomerContactNotFoundException(99),
                StatusCodes.Status404NotFound,
                "Customer contact not found",
                "Customer contact '99' was not found.",
                (Action<Exception>)(exception => ((CustomerContactNotFoundException)exception).CustomerId.ShouldBe(99))),
            new TestCaseData(
                () => (Exception)new BadHttpRequestException("Malformed"),
                StatusCodes.Status400BadRequest,
                "Invalid request",
                "Invalid request.",
                (Action<Exception>)(_ => { })),
            new TestCaseData(
                () => (Exception)new InvalidOperationException("Boom"),
                StatusCodes.Status500InternalServerError,
                "Unexpected error",
                "An unexpected error occurred while processing the request.",
                (Action<Exception>)(_ => { }))
        ];
    }
}
