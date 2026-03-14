using System.Net;
using System.Text.Json;

namespace Aida.ParallelChange.Api.Tests.Acceptance;

[TestFixture]
public sealed class SystemEndpointsAcceptanceTests : ApiAcceptanceFixture
{
    [Test]
    public async Task Health_endpoint_returns_ok_with_status_payload()
    {
        var response = await Client.GetAsync("/health");
        var body = await response.Content.ReadAsStringAsync();
        using var document = JsonDocument.Parse(body);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        document.RootElement.GetProperty("status").GetString().ShouldBe("ok");
    }

    [Test]
    public async Task OpenApi_endpoint_returns_v1_document_with_customer_contact_paths()
    {
        var response = await Client.GetAsync("/openapi/v1.json");
        var body = await response.Content.ReadAsStringAsync();
        using var document = JsonDocument.Parse(body);
        var paths = document.RootElement.GetProperty("paths");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        paths.TryGetProperty("/api/v1/customer-contacts", out _).ShouldBeTrue();
        paths.TryGetProperty("/api/v1/customer-contacts/{customerId}", out _).ShouldBeTrue();
    }
}
