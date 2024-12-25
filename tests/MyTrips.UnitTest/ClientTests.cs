using Bogus;
using FluentAssertions;
using FluentResults;
using Moq;
using MyTrips.Application.Services;
using MyTrips.Domain.Entities;
using MyTrips.Domain.Interfaces;

namespace MyTrips.UnitTest;

public class ClientTests
{
    private readonly IEnumerable<Client> _fakeClients = new Faker<Client>()
        .RuleFor(c => c.Name, f => f.Name.FullName())
        .RuleFor(c => c.Email, f => f.Internet.Email())
        .Generate(10);


    [Fact]
    public async Task GivenNonEmptyClientsTable_WhenGetClients_ItShouldReturnResultObjectWithThatClients()
    {
        // Arrange
        Mock<IClientsRepository> clientsRepositoryMock = new();
        clientsRepositoryMock.Setup(x => x.GetAsync()).ReturnsAsync(_fakeClients);
        var testResult = Result.Ok(_fakeClients);
        var clientsService = new ClientsService(clientsRepositoryMock.Object);

        // Act
        var clientsResult = await clientsService.GetClientsAsync();

        // Assert
        clientsResult.Should().BeEquivalentTo(testResult);
    }
}