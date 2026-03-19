using System.Net;
using System.Text.Json;
using Shouldly;

namespace Aida.ParallelChange.Api.Tests.Acceptance;

[TestFixture]
public sealed class ContractedCustomerContactV1AcceptanceTests : ApiAcceptanceFixture
{
    [Test]
    public async Task Legacy_v1_contract_remains_observable_after_create_update_and_get_flow()
    {
        const string createBody = """
        {
          "customerId": 91,
          "contactName": "Grace Hopper",
          "phone": "+1 5550100",
          "email": "grace.hopper@example.com"
        }
        """;

        const string updateBody = """
        {
          "contactName": "Grace Brewster Hopper",
          "phone": "+1 5550199",
          "email": "grace.b.hopper@example.com"
        }
        """;

        using var createRequest = CreateJsonApiRequest(HttpMethod.Post, "/api/v1/customer-contacts", createBody);
        using var updateRequest = CreateJsonApiRequest(HttpMethod.Put, "/api/v1/customer-contacts/91", updateBody);

        var postResponse = await Client.SendAsync(createRequest);
        var putResponse = await Client.SendAsync(updateRequest);
        var getResponse = await Client.GetAsync("/api/v1/customer-contacts/91");
        var getBody = await getResponse.Content.ReadAsStringAsync();
        using var document = JsonDocument.Parse(getBody);
        var attributes = document.RootElement.GetProperty("data").GetProperty("attributes");

        postResponse.StatusCode.ShouldBe(HttpStatusCode.Created);
        putResponse.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        getResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
        getResponse.Content.Headers.ContentType?.MediaType.ShouldBe(JsonApiMediaType);
        attributes.GetProperty("contactName").GetString().ShouldBe("Grace Brewster Hopper");
        attributes.GetProperty("phone").GetString().ShouldBe("+1 5550199");
        attributes.GetProperty("email").GetString().ShouldBe("grace.b.hopper@example.com");
    }
}
