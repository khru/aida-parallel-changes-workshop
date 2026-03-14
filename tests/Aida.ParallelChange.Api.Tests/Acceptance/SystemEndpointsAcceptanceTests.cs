using System.Net;
using System.Text.Json;

namespace Aida.ParallelChange.Api.Tests.Acceptance;

[TestFixture]
public sealed class SystemEndpointsAcceptanceTests
{
    private TestApiFactory _factory = null!;
    private HttpClient _client = null!;

    [SetUp]
    public void SetUp()
    {
        _factory = new TestApiFactory();
        _client = _factory.CreateClient();
    }

    [TearDown]
    public async Task TearDown()
    {
        _client.Dispose();
        await _factory.DisposeAsync();
    }

    [Test]
    public async Task Health_endpoint_returns_ok_with_status_payload()
    {
        var response = await _client.GetAsync("/health");
        var body = await response.Content.ReadAsStringAsync();
        using var document = JsonDocument.Parse(body);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        document.RootElement.GetProperty("status").GetString().ShouldBe("ok");
    }

    [Test]
    public async Task OpenApi_endpoint_returns_v1_document_with_customer_contact_paths()
    {
        var response = await _client.GetAsync("/openapi/v1.json");
        var body = await response.Content.ReadAsStringAsync();
        using var document = JsonDocument.Parse(body);
        var paths = document.RootElement.GetProperty("paths");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        paths.TryGetProperty("/api/v1/customer-contacts", out _).ShouldBeTrue();
        paths.TryGetProperty("/api/v1/customer-contacts/{customerId}", out _).ShouldBeTrue();
    }
}
