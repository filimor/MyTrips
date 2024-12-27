using Microsoft.AspNetCore.Mvc.Testing;
using MyTrips.IntegrationTests.Handlers;

namespace MyTrips.IntegrationTests.Fixtures;

public sealed class ClientsManagementFixture : IDisposable
{
    public string Endpoint = "https://localhost:5068/api/clients";
    public WebApplicationFactory<Program> Factory = new();

    public HttpClient Client =>
        Factory.CreateDefaultClient(new LoggingHandler { InnerHandler = new HttpClientHandler() });

    public void Dispose()
    {
        Client.Dispose();
        Factory.Dispose();
    }
}