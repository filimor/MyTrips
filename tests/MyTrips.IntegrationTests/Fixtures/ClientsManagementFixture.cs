using Bogus;
using Microsoft.AspNetCore.Mvc.Testing;
using MyTrips.Application.Dtos;
using MyTrips.Domain.Entities;
using MyTrips.IntegrationTests.Handlers;

namespace MyTrips.IntegrationTests.Fixtures;

public sealed class ClientsManagementFixture : IDisposable
{
    public Client ClientStub;
    public CreateClientDto CreateClientDtoStub;
    public string Endpoint = "https://localhost:5068/api/clients";
    public WebApplicationFactory<Program> Factory = new();
    public ResponseClientDto ResponseClientDtoStub;
    public UpdateClientDto UpdateClientDtoStub;

    public ClientsManagementFixture()
    {
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
}