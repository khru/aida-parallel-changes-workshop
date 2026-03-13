using System.Net;
using Shouldly;

namespace Aida.ParallelChange.Api.Tests.Acceptance;

[TestFixture]
public sealed class GetCustomerContactV1AcceptanceTests
{
    [Test]
    public async Task Get_returns_legacy_json_api_document()
    {
        await using var factory = new TestApiFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/v1/customer-contacts/42");
        var body = await response.Content.ReadAsStringAsync();

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        response.Content.Headers.ContentType?.MediaType.ShouldBe("application/vnd.api+json");
        body.ShouldContain("\"contactName\":\"María García\"");
        body.ShouldContain("\"phone\":\"+34 600123123\"");
        body.ShouldContain("\"email\":\"maria.garcia@example.com\"");
    }

    [Test]
    public async Task Get_returns_not_found_when_customer_does_not_exist()
    {
        await using var factory = new TestApiFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/v1/customer-contacts/9999");

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}
