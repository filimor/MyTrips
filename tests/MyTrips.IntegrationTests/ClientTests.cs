using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using MyTrips.Domain.Entities;
using Newtonsoft.Json;
using Serilog;
using Xunit.Abstractions;

namespace MyTrips.IntegrationTests;

public class ClientTests : IDisposable
{
    private readonly HttpClient _client;
    private readonly WebApplicationFactory<Program> _factory;
    private readonly ILogger _output;

    public ClientTests(ITestOutputHelper output)
    {
        _output = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .Enrich.WithExceptionData()
            .WriteTo.Console()
            .WriteTo.Debug()
            //.WriteTo.File("logs/test-log.txt", rollingInterval: RollingInterval.Day)
            .WriteTo.TestOutput(output)
            .CreateLogger()
            .ForContext<ClientTests>();

        _factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(
                    _ => { });
            });
        _client = _factory.CreateDefaultClient(new LoggingHandler { InnerHandler = new HttpClientHandler() });
    }

    public void Dispose()
    {
        _client.Dispose();
        _factory.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task GivenClientsEndpoint_WhenRequestedGet_ThenItShouldReturnOkWithHeadersAndContent()
    {
        // Arrange
        var client = _factory.CreateClient();
        var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost:5068/api/clients");

        // Act
        var response = await client.SendAsync(request);

        // Assert
        var responseString = await response.Content.ReadAsStringAsync();
        var returnedClients = JsonConvert.DeserializeObject<IEnumerable<Client>>(responseString);

        response.EnsureSuccessStatusCode();
        response.Content.Headers.ContentType?.ToString().Should().Be("application/json; charset=utf-8");
        returnedClients.Should().NotBeNull();
    }

    //[Fact]
    //public async Task
    //    GivenClientsEndpoint_WhenRequestedGetWithValidAndExistingId_ThenItShouldReturnOkWithHeadersAndContent()
    //{

    //}
}