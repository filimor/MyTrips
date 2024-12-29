using System.Net.Http.Headers;
using Bogus;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyTrips.Application.Dtos;
using MyTrips.Application.Interfaces;
using MyTrips.Application.Services;
using MyTrips.Domain.Entities;
using MyTrips.IntegrationTests.Handlers;

namespace MyTrips.IntegrationTests.Fixtures;

public sealed class ClientsManagementFixture : IDisposable
{
    private readonly IAuthService _authService;

    public Client ClientStub;
    public string ConnectionString;
    public CreateClientDto CreateClientDtoStub;
    public string Endpoint = "https://localhost:5068/api/clients";
    public WebApplicationFactory<Program> Factory = new();
    public ResponseClientDto ResponseClientDtoStub;
    public UpdateClientDto UpdateClientDtoStub;

    public ClientsManagementFixture()
    {
        var services = new ServiceCollection();
        services.AddTransient<IAuthService, AuthService>();
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.Development.json")
            .Build();
        services.AddSingleton<IConfiguration>(configuration);
        services.AddTransient<IAuthService, AuthService>();
        var serviceProvider = services.BuildServiceProvider();
        _authService = serviceProvider.GetRequiredService<IAuthService>();
        ConnectionString = configuration.GetConnectionString("MyTripsDb")!;

        var faker = new Faker<Client>();

        ClientStub = faker
            .RuleFor(c => c.Id, f => 1)
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

    public HttpClient DefaultHttpClient =>
        Factory.CreateDefaultClient(new LoggingHandler { InnerHandler = new HttpClientHandler() });


    public void Dispose()
    {
        DefaultHttpClient.Dispose();
        Factory.Dispose();
    }

    public AuthenticationHeaderValue GetAuthorizationHeader()
    {
        var token = _authService.GetToken(new LoginInfo { Username = "Admin", Password = "Password" });
        return new AuthenticationHeaderValue("Bearer", token);
    }
}