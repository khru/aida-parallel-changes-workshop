using System.Net;
using System.Text.Json;

namespace Aida.ParallelChange.Api.Tests.Acceptance;

[TestFixture]
public sealed class CustomerContactV1ContractAcceptanceTests : ApiAcceptanceFixture
{
    [Test]
    public async Task Error_response_uses_json_api_structure_for_bad_request()
    {
        var response = await Client.GetAsync("/api/v1/customer-contacts/invalid-id");
        var body = await response.Content.ReadAsStringAsync();
        using var document = JsonDocument.Parse(body);
        var error = document.RootElement.GetProperty("errors")[0];

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        response.Content.Headers.ContentType?.MediaType.ShouldBe("application/vnd.api+json");
        error.GetProperty("status").GetString().ShouldBe("400");
    }

    [Test]
    public async Task Error_response_uses_json_api_structure_for_not_found()
    {
        var response = await Client.GetAsync("/api/v1/customer-contacts/9999");
        var body = await response.Content.ReadAsStringAsync();
        using var document = JsonDocument.Parse(body);
        var error = document.RootElement.GetProperty("errors")[0];

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        response.Content.Headers.ContentType?.MediaType.ShouldBe("application/vnd.api+json");
        error.GetProperty("status").GetString().ShouldBe("404");
    }
}
