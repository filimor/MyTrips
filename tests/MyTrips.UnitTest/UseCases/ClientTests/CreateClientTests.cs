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

public class CreateClientTests
{
    private readonly Client _client;
    private readonly Mock<IClientsRepository> _clientsRepositoryMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly RequestClientDto _requestClientDto;
    private readonly ResponseClientDto _responseResponseClientDto;

    public CreateClientTests()
    {
        CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
        CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;

        _client = new Faker<Client>()
            .RuleFor(c => c.Name, f => f.Name.FullName())
            .RuleFor(c => c.Email, f => f.Internet.Email());

        _responseResponseClientDto = new ResponseClientDto
        {
            Name = _client.Name,
            Email = _client.Email
        };

        _requestClientDto = new RequestClientDto
        {
            Name = _client.Name,
            Email = _client.Email
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
        _clientsRepositoryMock.Setup(r => r.GetAsync(_client.Id)).ReturnsAsync(_client);
        var clientsService = new ClientsService(_mapperMock.Object, _clientsRepositoryMock.Object);
        var testResult = Result.Ok(_responseResponseClientDto);

        // Act
        var clientResult = await clientsService.AddNewClientAsync(_requestClientDto);

        // Assert
        clientResult.Should().BeEquivalentTo(testResult);
    }

    [Theory]
    [ClassData(typeof(InvalidStringClassData))]
    [Trait("Category", "Unit")]
    public async Task GivenInvalidName_WhenCreateClient_ThenItShouldReturnFailResultObjectWithTheErrors(string name)
    {
        // Arrange
        _requestClientDto.Name = name;
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
        var response = await controller.Post(_requestClientDto);

        // Assert
        response.Should().BeOfType<BadRequestObjectResult>()
            .Which.Value.Should().BeOfType<BadRequestErrorDetails>()
            .Which.Errors.Should().ContainMatch($"*{nameof(Client.Name)}*");
    }

    [Theory]
    [ClassData(typeof(InvalidEmailClassData))]
    [Trait("Category", "Unit")]
    public async Task GivenInvalidEmail_WhenCreateClient_ThenItShouldReturnFailResultObjectWithTheErrors(string email)
    {
        // Arrange
        _requestClientDto.Email = email;
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
        var response = await controller.Post(_requestClientDto);

        // Assert
        response.Should().BeOfType<BadRequestObjectResult>()
            .Which.Value.Should().BeOfType<BadRequestErrorDetails>()
            .Which.Errors.Should().ContainMatch($"*{nameof(Client.Email)}*");
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task GivenExistingEmail_WhenCreateClient_ThenItShouldReturnFailResultObjectWithTheErrors()
    {
        // Arrange
        var existingClient = new Faker<Client>()
            .RuleFor(c => c.Id, f => f.Random.Int())
            .RuleFor(c => c.Name, f => f.Name.FullName())
            .RuleFor(c => c.Email, f => _client.Email)
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
        var clientResult = await clientsService.AddNewClientAsync(_requestClientDto);

        // Assert
        clientResult.Should().BeEquivalentTo(testResult);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task GivenExistingEmail_WhenCreateClient_ThenItShouldReturnNotCreateIt()
    {
        // Arrange
        var existingClient = new Faker<Client>()
            .RuleFor(c => c.Id, f => f.Random.Int())
            .RuleFor(c => c.Name, f => f.Name.FullName())
            .RuleFor(c => c.Email, f => _client.Email)
            .Generate();
        var mockClientList = new List<Client> { existingClient };
        _clientsRepositoryMock.Setup(r => r.FindAsync(It.IsAny<Expression<Func<Client, bool>>>()))
            .ReturnsAsync((Expression<Func<Client, bool>> predicate) =>
                mockClientList.Where(predicate.Compile()));
        var clientsService = new ClientsService(_mapperMock.Object, _clientsRepositoryMock.Object);

        // Act
        await clientsService.AddNewClientAsync(_requestClientDto);

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
        var act = async () => await clientsService.AddNewClientAsync(_requestClientDto);
        // Assert
        await act.Should().ThrowAsync<OutOfMemoryException>();
    }
}