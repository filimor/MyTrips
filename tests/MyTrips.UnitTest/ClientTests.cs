using Bogus;
using FluentAssertions;
using Moq;

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
        Mock<IClientRepository> clientsRepositoryMock = new();
        clientsRepositoryMock.Setup(x => x.GetAsync()).ReturnsAsync(_fakeClients);
        var clientsService = new ClientsService(clientsRepositoryMock.Object);

        // Act
        var clients = await clientsService.GetClientsAsync();

        // Assert
        clients.Should().BeEquivalentTo(_fakeClients);
    }
}

public class ClientsService(IClientRepository clientRepository)
{
    public async Task<object> GetClientsAsync()
    {
        return await clientRepository.GetAsync();
    }
}

public interface IClientRepository
{
    Task<IEnumerable<Client>> GetAsync();
}

public class Client
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
}