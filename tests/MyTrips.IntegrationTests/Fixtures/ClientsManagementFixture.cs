using System.Net.Http.Headers;
using System.Text;
using Bogus;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyTrips.Application.Interfaces;
using MyTrips.Application.Services;
using MyTrips.Domain.ValueObjects;
using MyTrips.IntegrationTests.Handlers;
using Newtonsoft.Json;

namespace MyTrips.IntegrationTests.Fixtures;

public sealed class ClientsManagementFixture : IDisposable
{
    public const int MinId = 1;
    public const int InvalidId = -1;
    public const int NonExistentId = int.MaxValue;
    public string ConnectionString = null!;
    public string Endpoint = "https://localhost:5068/api/clients";

    public WebApplicationFactory<Program> Factory = new();

    public Client ClientStub = null!;
    public CreateClientDto CreateClientDtoStub = null!;
    public ResponseClientDto ResponseClientDtoStub = null!;
    public UpdateClientDto UpdateClientDtoStub = null!;
    private IAuthService _authService = null!;

    public ClientsManagementFixture()
    {
        InstantiateStubs();
        ConfigureServices();
    }

    public HttpClient DefaultHttpClient =>
        Factory.CreateDefaultClient(new LoggingHandler { InnerHandler = new HttpClientHandler() });


    public void Dispose()
    {
        DefaultHttpClient.Dispose();
        Factory.Dispose();
    }

    public HttpRequestMessage CreateRequest(HttpMethod method, object? entity = null, string? endpoint = null)
    {
        var request = entity is not null
            ? new HttpRequestMessage(method, endpoint ?? Endpoint)
            {
                Content = GetStringContent(entity)
            }
            : new HttpRequestMessage(method, endpoint ?? Endpoint);

        request.Headers.Authorization = GetAuthorizationHeader();

        return request;
    }

    public static StringContent GetStringContent(object entity)
    {
        var json = JsonConvert.SerializeObject(entity);
        return new StringContent(json, Encoding.UTF8, "application/json");
    }

    public AuthenticationHeaderValue GetAuthorizationHeader()
    {
        var token = _authService.GetToken(new LoginInfo { Username = "Admin", Password = "Password" });
        return new AuthenticationHeaderValue("Bearer", token);
    }

    private void ConfigureServices()
    {
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.Development.json")
            .Build();

        services.AddTransient<IAuthService, AuthService>();
        services.AddSingleton<IConfiguration>(configuration);
        services.AddTransient<IAuthService, AuthService>();

        var serviceProvider = services.BuildServiceProvider();
        _authService = serviceProvider.GetRequiredService<IAuthService>();
        ConnectionString = configuration.GetConnectionString("MyTripsDb")!;
    }

    private void InstantiateStubs()
    {
        var faker = new Faker<Client>();

        ClientStub = faker
            .RuleFor(c => c.Id, _ => 1)
            .RuleFor(c => c.Name, f => f.Name.FullName())
            .RuleFor(c => c.Email, f => f.Internet.Email());

        ResponseClientDtoStub = new ResponseClientDto
        {
            Id = ClientStub.Id,
            Name = ClientStub.Name,
            Email = ClientStub.Email
        };

        UpdateClientDtoStub = new UpdateClientDto
        {
            Id = ClientStub.Id,
            Name = ClientStub.Name,
            Email = ClientStub.Email
        };

        CreateClientDtoStub = new CreateClientDto
        {
            Name = ClientStub.Name,
            Email = ClientStub.Email
        };
    }
}