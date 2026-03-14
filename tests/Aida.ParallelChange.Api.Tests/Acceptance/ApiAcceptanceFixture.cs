using System.Text;

namespace Aida.ParallelChange.Api.Tests.Acceptance;

public abstract class ApiAcceptanceFixture
{
    protected const string JsonApiMediaType = "application/vnd.api+json";

    private TestApiFactory _factory = null!;

    protected HttpClient Client = null!;

    [SetUp]
    public virtual void SetUp()
    {
        _factory = new TestApiFactory();
        Client = _factory.CreateClient();
    }

    [TearDown]
    public virtual async Task TearDown()
    {
        Client.Dispose();
        await _factory.DisposeAsync();
    }

    protected static HttpRequestMessage CreateJsonApiRequest(HttpMethod method, string uri, string body)
    {
        return new HttpRequestMessage(method, uri)
        {
            Content = new StringContent(body, Encoding.UTF8, JsonApiMediaType)
        };
    }
}
