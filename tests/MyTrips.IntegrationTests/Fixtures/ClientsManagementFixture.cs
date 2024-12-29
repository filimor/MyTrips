using System.Net.Http.Headers;
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

    public AuthenticationHeaderValue GetAuthorizationHeader()
    {
        return new AuthenticationHeaderValue("Bearer",
            "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6ImZpbGltIiwic3ViIjoiZmlsaW0iLCJqdGkiOiI3OTE0ZjFhYyIsImF1ZCI6WyJodHRwOi8vbG9jYWxob3N0OjY0OTk1IiwiaHR0cHM6Ly9sb2NhbGhvc3Q6NDQzNTIiLCJodHRwOi8vbG9jYWxob3N0OjUwMDgiLCJodHRwczovL2xvY2FsaG9zdDo3Mjg2Il0sIm5iZiI6MTczNTQ0NzQzNSwiZXhwIjoxNzQzMjIzNDM1LCJpYXQiOjE3MzU0NDc0MzgsImlzcyI6ImRvdG5ldC11c2VyLWp3dHMifQ.zzKaL-A3dITMTMovEjuuLXMUcU6FXm90dYKzAPxNCRg");
    }
}