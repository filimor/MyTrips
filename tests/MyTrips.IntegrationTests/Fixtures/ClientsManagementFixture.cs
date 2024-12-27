using Bogus;
using Microsoft.AspNetCore.Mvc.Testing;
using MyTrips.Application.Dtos;
using MyTrips.Domain.Entities;
using MyTrips.IntegrationTests.Handlers;

namespace MyTrips.IntegrationTests.Fixtures;

public sealed class ClientsManagementFixture : IDisposable
{
    public Client ClientStub;
    public string Endpoint = "https://localhost:5068/api/clients";
    public WebApplicationFactory<Program> Factory = new();
    public RequestClientDto RequestClientDtoStub;
    public ResponseClientDto ResponseClientDtoStub;

    public ClientsManagementFixture()
    {
        ClientStub = new Faker<Client>()
            .RuleFor(c => c.Name, f => f.Name.FullName())
            .RuleFor(c => c.Email, f => f.Internet.Email());

        ResponseClientDtoStub = new ResponseClientDto
        {
            Name = ClientStub.Name,
            Email = ClientStub.Email
        };

        RequestClientDtoStub = new RequestClientDto
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
}