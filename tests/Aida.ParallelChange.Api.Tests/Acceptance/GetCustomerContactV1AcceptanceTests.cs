using System.Net;
using System.Text.Json;
using Shouldly;

namespace Aida.ParallelChange.Api.Tests.Acceptance;

[TestFixture]
public sealed class GetCustomerContactV1AcceptanceTests : SeededCustomerContactAcceptanceFixture
{
    [Test]
    public async Task Get_returns_legacy_json_api_document()
    {
        var response = await Client.GetAsync("/api/v1/customer-contacts/42");
        var body = await response.Content.ReadAsStringAsync();
        using var document = JsonDocument.Parse(body);
        var attributes = document.RootElement.GetProperty("data").GetProperty("attributes");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        response.Content.Headers.ContentType?.MediaType.ShouldBe("application/vnd.api+json");
        attributes.GetProperty("contactName").GetString().ShouldBe("Maria Garcia");
        attributes.GetProperty("phone").GetString().ShouldBe("+34 600123123");
        attributes.GetProperty("email").GetString().ShouldBe("maria.garcia@example.com");
    }

    [Test]
    public async Task Get_returns_not_found_when_customer_does_not_exist()
    {
        var response = await Client.GetAsync("/api/v1/customer-contacts/9999");
        var body = await response.Content.ReadAsStringAsync();
        using var document = JsonDocument.Parse(body);
        var error = document.RootElement.GetProperty("errors")[0];

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        response.Content.Headers.ContentType?.MediaType.ShouldBe("application/vnd.api+json");
        error.GetProperty("status").GetString().ShouldBe("404");
        error.GetProperty("title").GetString().ShouldBe("Customer contact not found");
        error.GetProperty("detail").GetString().ShouldBe("Customer contact '9999' was not found.");
    }

    [Test]
    public async Task Get_returns_bad_request_when_customer_id_is_invalid()
    {
        var response = await Client.GetAsync("/api/v1/customer-contacts/invalid-id");
        var body = await response.Content.ReadAsStringAsync();
        using var document = JsonDocument.Parse(body);
        var error = document.RootElement.GetProperty("errors")[0];

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        response.Content.Headers.ContentType?.MediaType.ShouldBe("application/vnd.api+json");
        error.GetProperty("status").GetString().ShouldBe("400");
        error.GetProperty("title").GetString().ShouldBe("Invalid customer id");
        error.GetProperty("detail").GetString().ShouldBe("Customer id must be a number greater than zero.");
    }

    [Test]
    public async Task Get_returns_bad_request_when_customer_id_is_zero()
    {
        var response = await Client.GetAsync("/api/v1/customer-contacts/0");
        var body = await response.Content.ReadAsStringAsync();
        using var document = JsonDocument.Parse(body);
        var error = document.RootElement.GetProperty("errors")[0];

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        response.Content.Headers.ContentType?.MediaType.ShouldBe("application/vnd.api+json");
        error.GetProperty("status").GetString().ShouldBe("400");
        error.GetProperty("title").GetString().ShouldBe("Invalid customer id");
        error.GetProperty("detail").GetString().ShouldStartWith("Customer id must be greater than zero.");
    }
}
