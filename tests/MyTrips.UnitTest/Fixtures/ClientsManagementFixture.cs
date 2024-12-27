using System.Globalization;
using AutoMapper;
using Bogus;
using Moq;
using MyTrips.Application.Dtos;
using MyTrips.Domain.Entities;
using MyTrips.Domain.Interfaces;

namespace MyTrips.UnitTest.Fixtures;

public sealed class ClientsManagementFixture
{
    public readonly Mock<IClientsRepository> ClientsRepositoryMock = new();
    public readonly Mock<IMapper> MapperMock = new();
    public IEnumerable<Client> ClientsCollectionStub = null!;
    public Client ClientStub = null!;
    public CreateClientDto CreateClientDtoStub = null!;
    public ResponseClientDto ResponseClientDtoStub = null!;
    public UpdateClientDto UpdateClientDtoStub = null!;

    public ClientsManagementFixture()
    {
        SetCulture();
        InstantiateStubs();
        SetupMocks();
    }

    // TODO: Check whether this method is still necessary
    private static void SetCulture()
    {
        CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
        CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;
    }

    private void InstantiateStubs()
    {
        var faker = new Faker<Client>();

        ClientStub = faker
            .RuleFor(c => c.Id, f => 1)
            .RuleFor(c => c.Name, f => f.Name.FullName())
            .RuleFor(c => c.Email, f => f.Internet.Email());

        ClientsCollectionStub = faker
            .RuleFor(c => c.Name, f => f.Name.FullName())
            .RuleFor(c => c.Email, f => f.Internet.Email())
            .Generate(10);

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

    private void SetupMocks()
    {
        MapperMock.Setup(m => m.Map<ResponseClientDto>(It.IsAny<Client>()))
            .Returns((Client client) => new ResponseClientDto
            {
                Id = client.Id,
                Name = client.Name,
                Email = client.Email
            });

        MapperMock.Setup(m => m.Map<IEnumerable<ResponseClientDto>>(It.IsAny<IEnumerable<Client>>()))
            .Returns((IEnumerable<Client> clients) => clients.Select(c => new ResponseClientDto
            {
                Id = c.Id,
                Name = c.Name,
                Email = c.Email
            }));

        MapperMock.Setup(m => m.Map<Client>(It.IsAny<CreateClientDto>()))
            .Returns(
                (CreateClientDto requestClientDto) => new Client(requestClientDto.Name, requestClientDto.Email));

        MapperMock.Setup(m => m.Map<Client>(It.IsAny<UpdateClientDto>()))
            .Returns(
                (UpdateClientDto updateClientDto) =>
                    new Client(updateClientDto.Id, updateClientDto.Name, updateClientDto.Email));

        MapperMock.Setup(m => m.Map<ResponseClientDto>(It.IsAny<Client>()))
            .Returns((Client client) => new ResponseClientDto
            {
                Id = client.Id,
                Name = client.Name,
                Email = client.Email
            });

        ClientsRepositoryMock.Setup(r => r.GetAsync(ClientStub.Id)).ReturnsAsync(ClientStub);
        ClientsRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Client>())).ReturnsAsync(ClientStub.Id);
        ClientsRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Client>()))
            .ReturnsAsync((Client client) => client);
    }
}