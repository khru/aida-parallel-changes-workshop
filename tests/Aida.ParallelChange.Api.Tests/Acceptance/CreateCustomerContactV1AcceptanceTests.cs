using System.Net;
using System.Text;
using System.Text.Json;
using Shouldly;

namespace Aida.ParallelChange.Api.Tests.Acceptance;

[TestFixture]
public sealed class CreateCustomerContactV1AcceptanceTests
{
    private const string JsonApiMediaType = "application/vnd.api+json";

    private TestApiFactory _factory = null!;
    private HttpClient _client = null!;

    [SetUp]
    public async Task SetUp()
    {
        _factory = new TestApiFactory();
        _client = _factory.CreateClient();

        const string body = """
        {
          "customerId": 42,
          "contactName": "Maria Garcia",
          "phone": "+34 600123123",
          "email": "maria.garcia@example.com"
        }
        """;

        using var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/customer-contacts")
        {
            Content = new StringContent(body, Encoding.UTF8, JsonApiMediaType)
        };

        var response = await _client.SendAsync(request);

        response.StatusCode.ShouldBe(HttpStatusCode.Created);
    }

    [TearDown]
    public async Task TearDown()
    {
        _client.Dispose();
        await _factory.DisposeAsync();
    }

    [Test]
    public async Task Post_creates_customer_contact_through_legacy_contract()
    {
        const string body = """
        {
          "customerId": 77,
          "contactName": "Grace Hopper",
          "phone": "+1 5550100",
          "email": "grace.hopper@example.com"
        }
        """;

        using var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/customer-contacts")
        {
            Content = new StringContent(body, Encoding.UTF8, JsonApiMediaType)
        };

        var postResponse = await _client.SendAsync(request);
        var getResponse = await _client.GetAsync("/api/v1/customer-contacts/77");
        var getBody = await getResponse.Content.ReadAsStringAsync();
        using var document = JsonDocument.Parse(getBody);
        var attributes = document.RootElement.GetProperty("data").GetProperty("attributes");

        postResponse.StatusCode.ShouldBe(HttpStatusCode.Created);
        postResponse.Headers.Location.ShouldNotBeNull();
        postResponse.Headers.Location!.OriginalString.ShouldBe("/api/v1/customer-contacts/77");
        getResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
        attributes.GetProperty("contactName").GetString().ShouldBe("Grace Hopper");
        attributes.GetProperty("phone").GetString().ShouldBe("+1 5550100");
        attributes.GetProperty("email").GetString().ShouldBe("grace.hopper@example.com");
    }

    [Test]
    public async Task Post_returns_bad_request_when_payload_is_invalid()
    {
        const string body = """
        {
          "customerId": 78,
          "contactName": "",
          "phone": "+1 5550101",
          "email": "invalid"
        }
        """;

        using var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/customer-contacts")
        {
            Content = new StringContent(body, Encoding.UTF8, JsonApiMediaType)
        };

        var response = await _client.SendAsync(request);
        var responseBody = await response.Content.ReadAsStringAsync();
        using var document = JsonDocument.Parse(responseBody);
        var error = document.RootElement.GetProperty("errors")[0];

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        error.GetProperty("status").GetString().ShouldBe("400");
        error.GetProperty("title").GetString().ShouldBe("Invalid request");
        error.GetProperty("detail").GetString().ShouldBe("Contact name is required. (Parameter 'value')");
    }

    [Test]
    public async Task Post_returns_conflict_when_customer_already_exists()
    {
        const string body = """
        {
          "customerId": 42,
          "contactName": "Existing",
          "phone": "+1 5550102",
          "email": "existing@example.com"
        }
        """;

        using var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/customer-contacts")
        {
            Content = new StringContent(body, Encoding.UTF8, JsonApiMediaType)
        };

        var response = await _client.SendAsync(request);
        var responseBody = await response.Content.ReadAsStringAsync();
        using var document = JsonDocument.Parse(responseBody);
        var error = document.RootElement.GetProperty("errors")[0];

        response.StatusCode.ShouldBe(HttpStatusCode.Conflict);
        error.GetProperty("status").GetString().ShouldBe("409");
        error.GetProperty("title").GetString().ShouldBe("Customer contact already exists");
        error.GetProperty("detail").GetString().ShouldBe("Customer contact '42' already exists.");
    }

    [TestCase("{\"customerId\":79,\"phone\":\"+1 5550101\",\"email\":\"name.missing@example.com\"}", "Contact name is required. (Parameter 'value')")]
    [TestCase("{\"customerId\":80,\"contactName\":\"Phone Missing\",\"email\":\"phone.missing@example.com\"}", "Phone number is required. (Parameter 'value')")]
    [TestCase("{\"customerId\":81,\"contactName\":\"Email Missing\",\"phone\":\"+1 5550103\"}", "Email address is invalid. (Parameter 'value')")]
    public async Task Post_returns_bad_request_when_required_field_is_missing(string body, string expectedDetail)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/customer-contacts")
        {
            Content = new StringContent(body, Encoding.UTF8, JsonApiMediaType)
        };

        var response = await _client.SendAsync(request);
        var responseBody = await response.Content.ReadAsStringAsync();
        using var document = JsonDocument.Parse(responseBody);
        var error = document.RootElement.GetProperty("errors")[0];

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        error.GetProperty("detail").GetString().ShouldBe(expectedDetail);
    }
}
