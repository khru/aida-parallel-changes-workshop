using System.Net;
using System.Text;
using System.Text.Json;
using Shouldly;

namespace Aida.ParallelChange.Api.Tests.Acceptance;

[TestFixture]
public sealed class UpdateCustomerContactV1AcceptanceTests
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
    public async Task Put_updates_customer_contact_through_legacy_contract()
    {
        const string body = """
        {
          "contactName": "Ada Lovelace",
          "phone": "+44 123456789",
          "email": "ada.lovelace@example.com"
        }
        """;

        using var request = new HttpRequestMessage(HttpMethod.Put, "/api/v1/customer-contacts/42")
        {
            Content = new StringContent(body, Encoding.UTF8, JsonApiMediaType)
        };

        var putResponse = await _client.SendAsync(request);
        var getResponse = await _client.GetAsync("/api/v1/customer-contacts/42");
        var getBody = await getResponse.Content.ReadAsStringAsync();
        using var document = JsonDocument.Parse(getBody);
        var attributes = document.RootElement.GetProperty("data").GetProperty("attributes");

        putResponse.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        getResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
        attributes.GetProperty("contactName").GetString().ShouldBe("Ada Lovelace");
        attributes.GetProperty("phone").GetString().ShouldBe("+44 123456789");
        attributes.GetProperty("email").GetString().ShouldBe("ada.lovelace@example.com");
    }

    [Test]
    public async Task Put_returns_not_found_when_customer_does_not_exist()
    {
        const string body = """
        {
          "contactName": "Ada Lovelace",
          "phone": "+44 123456789",
          "email": "ada.lovelace@example.com"
        }
        """;

        using var request = new HttpRequestMessage(HttpMethod.Put, "/api/v1/customer-contacts/9999")
        {
            Content = new StringContent(body, Encoding.UTF8, JsonApiMediaType)
        };

        var response = await _client.SendAsync(request);
        var responseBody = await response.Content.ReadAsStringAsync();
        using var document = JsonDocument.Parse(responseBody);
        var error = document.RootElement.GetProperty("errors")[0];

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        error.GetProperty("status").GetString().ShouldBe("404");
        error.GetProperty("title").GetString().ShouldBe("Customer contact not found");
        error.GetProperty("detail").GetString().ShouldBe("Customer contact '9999' was not found.");
    }

    [Test]
    public async Task Put_returns_bad_request_when_customer_id_is_invalid()
    {
        const string body = """
        {
          "contactName": "Ada Lovelace",
          "phone": "+44 123456789",
          "email": "ada.lovelace@example.com"
        }
        """;

        using var request = new HttpRequestMessage(HttpMethod.Put, "/api/v1/customer-contacts/invalid")
        {
            Content = new StringContent(body, Encoding.UTF8, JsonApiMediaType)
        };

        var response = await _client.SendAsync(request);
        var responseBody = await response.Content.ReadAsStringAsync();
        using var document = JsonDocument.Parse(responseBody);
        var error = document.RootElement.GetProperty("errors")[0];

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        error.GetProperty("status").GetString().ShouldBe("400");
        error.GetProperty("title").GetString().ShouldBe("Invalid customer id");
        error.GetProperty("detail").GetString().ShouldBe("Customer id must be a number greater than zero.");
    }

    [Test]
    public async Task Put_returns_bad_request_when_customer_id_is_zero()
    {
        const string body = """
        {
          "contactName": "Ada Lovelace",
          "phone": "+44 123456789",
          "email": "ada.lovelace@example.com"
        }
        """;

        using var request = new HttpRequestMessage(HttpMethod.Put, "/api/v1/customer-contacts/0")
        {
            Content = new StringContent(body, Encoding.UTF8, JsonApiMediaType)
        };

        var response = await _client.SendAsync(request);
        var responseBody = await response.Content.ReadAsStringAsync();
        using var document = JsonDocument.Parse(responseBody);
        var error = document.RootElement.GetProperty("errors")[0];

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        error.GetProperty("status").GetString().ShouldBe("400");
        error.GetProperty("title").GetString().ShouldBe("Invalid customer id");
        error.GetProperty("detail").GetString().ShouldStartWith("Customer id must be greater than zero.");
    }

    [Test]
    public async Task Put_returns_bad_request_when_payload_is_invalid()
    {
        const string body = """
        {
          "contactName": "",
          "phone": "+44 123456789",
          "email": "invalid"
        }
        """;

        using var request = new HttpRequestMessage(HttpMethod.Put, "/api/v1/customer-contacts/42")
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

    [TestCase("{\"phone\":\"+44 123456789\",\"email\":\"ada.lovelace@example.com\"}", "Contact name is required. (Parameter 'value')")]
    [TestCase("{\"contactName\":\"Ada Lovelace\",\"email\":\"ada.lovelace@example.com\"}", "Phone number is required. (Parameter 'value')")]
    [TestCase("{\"contactName\":\"Ada Lovelace\",\"phone\":\"+44 123456789\"}", "Email address is invalid. (Parameter 'value')")]
    public async Task Put_returns_bad_request_when_required_field_is_missing(string body, string expectedDetail)
    {
        using var request = new HttpRequestMessage(HttpMethod.Put, "/api/v1/customer-contacts/42")
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
