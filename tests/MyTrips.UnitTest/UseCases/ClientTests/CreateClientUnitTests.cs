using System.Globalization;
using System.Linq.Expressions;
using AutoMapper;
using Bogus;
using FluentAssertions;
using FluentResults;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MyTrips.Application.Dtos;
using MyTrips.Application.Interfaces;
using MyTrips.Application.Services;
using MyTrips.Application.Validators;
using MyTrips.Domain.Entities;
using MyTrips.Domain.Interfaces;
using MyTrips.Presentation.Controllers;
using MyTrips.Presentation.Errors;
using MyTrips.UnitTest.ClassData;

namespace MyTrips.UnitTest.UseCases.ClientTests;

public class CreateClientUnitTests
{
    private readonly Mock<IClientsRepository> _clientsRepositoryMock = new();
    private readonly Client _clientStub;
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly RequestClientDto _requestClientDtoStub;
    private readonly ResponseClientDto _responseResponseClientDtoStub;

    public CreateClientUnitTests()
    {
        CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
        CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;

        _clientStub = new Faker<Client>()
            .RuleFor(c => c.Name, f => f.Name.FullName())
            .RuleFor(c => c.Email, f => f.Internet.Email());

        _responseResponseClientDtoStub = new ResponseClientDto
        {
            Name = _clientStub.Name,
            Email = _clientStub.Email
        };

        _requestClientDtoStub = new RequestClientDto
        {
            Name = _clientStub.Name,
            Email = _clientStub.Email
        };

        _mapperMock.Setup(m => m.Map<Client>(It.IsAny<RequestClientDto>()))
            .Returns(
                (RequestClientDto requestClientDto) => new Client(requestClientDto.Name, requestClientDto.Email));

        _mapperMock.Setup(m => m.Map<ResponseClientDto>(It.IsAny<Client>()))
            .Returns((Client client) => new ResponseClientDto
            {
                Id = client.Id,
                Name = client.Name,
                Email = client.Email
            });
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task GivenValidClient_WhenCreateClient_ThenItShouldReturnOkResultObjectWithTheDto()
    {
        // Arrange
        _clientsRepositoryMock.Setup(r => r.GetAsync(_clientStub.Id)).ReturnsAsync(_clientStub);
        var clientsService = new ClientsService(_mapperMock.Object, _clientsRepositoryMock.Object);
        var testResult = Result.Ok(_responseResponseClientDtoStub);

        // Act
        var clientResult = await clientsService.AddNewClientAsync(_requestClientDtoStub);

        // Assert
        clientResult.Should().BeEquivalentTo(testResult);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task GivenValidClient_WhenCreateClient_ThenItShouldBePersisted()
    {
        // Arrange
        _clientsRepositoryMock.Setup(r => r.GetAsync(_clientStub.Id)).ReturnsAsync(_clientStub);
        var clientsService = new ClientsService(_mapperMock.Object, _clientsRepositoryMock.Object);
        var testResult = Result.Ok(_responseResponseClientDtoStub);

        // Act
        var clientResult = await clientsService.AddNewClientAsync(_requestClientDtoStub);

        // Assert
        _clientsRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Client>()), Times.Once);
    }

    [Theory]
    [ClassData(typeof(InvalidStringClassData))]
    [Trait("Category", "Unit")]
    public async Task GivenInvalidName_WhenTryToCreateClient_ThenItShouldReturnFailResultObjectWithTheErrors(
        string name)
    {
        // Arrange
        _requestClientDtoStub.Name = name;
        var clientServiceMock = new Mock<IClientsService>();

        var httpContext = new DefaultHttpContext();
        var controllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };
        var controller = new ClientsController(clientServiceMock.Object, new ClientValidator())
        {
            ControllerContext = controllerContext
        };

        // Act
        var response = await controller.Post(_requestClientDtoStub);

        // Assert
        response.Should().BeOfType<BadRequestObjectResult>()
            .Which.Value.Should().BeOfType<BadRequestErrorDetails>()
            .Which.Errors.Should().ContainMatch($"*{nameof(Client.Name)}*");
    }

    [Theory]
    [ClassData(typeof(InvalidEmailClassData))]
    [Trait("Category", "Unit")]
    public async Task GivenInvalidEmail_WhenTryToCreateClient_ThenItShouldReturnFailResultObjectWithTheErrors(
        string email)
    {
        // Arrange
        _requestClientDtoStub.Email = email;
        var clientServiceMock = new Mock<IClientsService>();

        var httpContext = new DefaultHttpContext();
        var controllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };
        var controller = new ClientsController(clientServiceMock.Object, new ClientValidator())
        {
            ControllerContext = controllerContext
        };

        // Act
        var response = await controller.Post(_requestClientDtoStub);

        // Assert
        response.Should().BeOfType<BadRequestObjectResult>()
            .Which.Value.Should().BeOfType<BadRequestErrorDetails>()
            .Which.Errors.Should().ContainMatch($"*{nameof(Client.Email)}*");
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task GivenExistingEmail_WhenTryToCreateClient_ThenItShouldReturnFailResultObjectWithTheErrors()
    {
        // Arrange
        var existingClient = new Faker<Client>()
            .RuleFor(c => c.Id, f => f.Random.Int())
            .RuleFor(c => c.Name, f => f.Name.FullName())
            .RuleFor(c => c.Email, f => _clientStub.Email)
            .Generate();
        var mockClientList = new List<Client> { existingClient };
        _clientsRepositoryMock.Setup(r => r.FindAsync(It.IsAny<Expression<Func<Client, bool>>>()))
            .ReturnsAsync((Expression<Func<Client, bool>> predicate) =>
                mockClientList.Where(predicate.Compile()));
        var clientsService = new ClientsService(_mapperMock.Object, _clientsRepositoryMock.Object);
        var testResult =
            Result.Fail(new Error(
                $"{nameof(Client)} with the {nameof(Client.Email)} '{existingClient.Email}' already exists."));

        // Act
        var clientResult = await clientsService.AddNewClientAsync(_requestClientDtoStub);

        // Assert
        clientResult.Should().BeEquivalentTo(testResult);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task GivenExistingEmail_WhenTryToCreateClient_ThenItShouldReturnNotPersistIt()
    {
        // Arrange
        var existingClient = new Faker<Client>()
            .RuleFor(c => c.Id, f => f.Random.Int())
            .RuleFor(c => c.Name, f => f.Name.FullName())
            .RuleFor(c => c.Email, f => _clientStub.Email)
            .Generate();
        var mockClientList = new List<Client> { existingClient };
        _clientsRepositoryMock.Setup(r => r.FindAsync(It.IsAny<Expression<Func<Client, bool>>>()))
            .ReturnsAsync((Expression<Func<Client, bool>> predicate) =>
                mockClientList.Where(predicate.Compile()));
        var clientsService = new ClientsService(_mapperMock.Object, _clientsRepositoryMock.Object);

        // Act
        await clientsService.AddNewClientAsync(_requestClientDtoStub);

        // Assert
        _clientsRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Client>()), Times.Never);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task GivenCreateRequest_WhenRepositoryThrowException_ThenItShouldThrowTheException()
    {
        // Arrange
        _clientsRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Client>())).ThrowsAsync(new OutOfMemoryException());
        var clientsService = new ClientsService(_mapperMock.Object, _clientsRepositoryMock.Object);
        // Act
        var act = async () => await clientsService.AddNewClientAsync(_requestClientDtoStub);
        // Assert
        await act.Should().ThrowAsync<OutOfMemoryException>();
    }
}