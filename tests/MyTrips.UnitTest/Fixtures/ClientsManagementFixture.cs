using System.Globalization;
using AutoMapper;
using FluentValidation;
using MyTrips.Application.Dtos;
using MyTrips.Application.Interfaces;
using MyTrips.Application.Validators;
using MyTrips.Domain.Entities;
using MyTrips.Domain.Interfaces;
using MyTrips.Presentation.Controllers;

namespace MyTrips.UnitTest.Fixtures;

public sealed class ClientsManagementFixture
{
    public readonly Mock<IClientsRepository> ClientsRepositoryMock = new();
    public readonly Mock<IMapper> MapperMock = new();
    public Mock<IClientsService> ClientServiceMock = new();

    public IEnumerable<Client> ClientsCollectionStub = null!;
    public Client ClientStub = null!;
    public Client OtherClientStub = null!;
    public CreateClientDto CreateClientDtoStub = null!;
    public ResponseClientDto ResponseClientDtoStub = null!;
    public UpdateClientDto UpdateClientDtoStub = null!;
    public List<Client> SearchClientResultStub = null!;

    public ClientsManagementFixture()
    {
        SetCulture();
        InstantiateStubs();
        SetupMocks();
    }

    public ClientsController NewClientsController(IValidator<Client>? clientValidator = null)
    {
        clientValidator ??= new ClientValidator();
        var httpContext = new DefaultHttpContext();

        var controllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };

        var controller = new ClientsController(ClientServiceMock.Object, clientValidator)
        {
            ControllerContext = controllerContext
        };

        return controller;
    }


    private static void SetCulture()
    {
        CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
        CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;
    }

    private void InstantiateStubs()
    {
        var faker = new Faker<Client>();

        ClientStub = faker
            .RuleFor(c => c.Id, _ => 1)
            .RuleFor(c => c.Name, f => f.Name.FullName())
            .RuleFor(c => c.Email, f => f.Internet.Email());

        ClientsCollectionStub = faker
            .RuleFor(c => c.Name, f => f.Name.FullName())
            .RuleFor(c => c.Email, f => f.Internet.Email())
            .Generate(10);

        OtherClientStub = faker
            .RuleFor(c => c.Id, _ => 1)
            .RuleFor(c => c.Name, f => f.Name.FullName())
            .RuleFor(c => c.Email, f => f.Internet.Email());

        SearchClientResultStub = [OtherClientStub];

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
        ClientsRepositoryMock.Setup(r => r.DeleteAsync(ClientStub.Id)).ReturnsAsync(1);
        ClientsRepositoryMock.Setup(r => r.FindAsync(It.IsAny<Expression<Func<Client, bool>>>()))
            .ReturnsAsync((Expression<Func<Client, bool>> predicate) =>
                SearchClientResultStub.Where(predicate.Compile()));
    }
}