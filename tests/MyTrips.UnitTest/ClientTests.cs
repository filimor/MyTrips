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
    private readonly Mock<IClientsRepository> _clientsRepositoryMock = new();

    private readonly IEnumerable<Client> _fakeClients = new Faker<Client>()
        .RuleFor(c => c.Name, f => f.Name.FullName())
        .RuleFor(c => c.Email, f => f.Internet.Email())
        .Generate(10);

    [Fact]
    public async Task GivenNonEmptyClientsTable_WhenGetClients_ItShouldReturnResultObjectWithThatClients()
    {
        // Arrange
        _clientsRepositoryMock.Setup(r => r.GetAsync()).ReturnsAsync(_fakeClients);
        var testResult = Result.Ok(_fakeClients);
        var clientsService = new ClientsService(_clientsRepositoryMock.Object);

        // Act
        var clientsResult = await clientsService.GetClientsAsync();

        // Assert
        clientsResult.Should().BeEquivalentTo(testResult);
    }

    [Fact]
    public async Task GivenGetRequest_WhenRepositoryThrowException_ItShouldReturnServerErrorResultObject()
    {
        // Arrange
        _clientsRepositoryMock.Setup(r => r.GetAsync()).ThrowsAsync(new OutOfMemoryException());
        var clientsService = new ClientsService(_clientsRepositoryMock.Object);

        // Act
        var act = () => clientsService.GetClientsAsync();

        // Assert
        await act.Should().ThrowAsync<OutOfMemoryException>();
    }
}