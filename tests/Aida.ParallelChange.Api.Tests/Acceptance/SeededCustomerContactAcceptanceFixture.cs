using System.Net;

namespace Aida.ParallelChange.Api.Tests.Acceptance;

public abstract class SeededCustomerContactAcceptanceFixture : ApiAcceptanceFixture
{
    private const string SeedPayload = """
    {
      "customerId": 42,
      "contactName": "Maria Garcia",
      "phone": "+34 600123123",
      "email": "maria.garcia@example.com"
    }
    """;

    [SetUp]
    public async Task SeedDefaultCustomerContact()
    {
        using var request = CreateJsonApiRequest(HttpMethod.Post, "/api/v1/customer-contacts", SeedPayload);
        var response = await Client.SendAsync(request);
        response.StatusCode.ShouldBe(HttpStatusCode.Created);
    }
}
