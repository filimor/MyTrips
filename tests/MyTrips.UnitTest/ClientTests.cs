using System.Globalization;
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
using MyTrips.Domain.Entities;
using MyTrips.Domain.Interfaces;
using MyTrips.Presentation.Controllers;
using MyTrips.Presentation.Errors;

namespace MyTrips.UnitTest;

public class ClientTests
{
    private readonly Mock<IClientsRepository> _clientsRepositoryMock = new();

    private readonly IEnumerable<Client> _fakeClients = new Faker<Client>()
        .RuleFor(c => c.Name, f => f.Name.FullName())
        .RuleFor(c => c.Email, f => f.Internet.Email())
        .Generate(10);

    private readonly Mock<IMapper> _mapperMock = new();

    public ClientTests()
    {
        CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
        CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;

        _mapperMock.Setup(m => m.Map<IEnumerable<ClientDto>>(It.IsAny<IEnumerable<Client>>()))
            .Returns((IEnumerable<Client> clients) => clients.Select(c => new ClientDto
            {
                Id = c.Id,
                Name = c.Name,
                Email = c.Email
            }));
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task GivenNonEmptyClientsTable_WhenGetClients_ThenItShouldReturnResultObjectWithThatClients()
    {
        // Arrange
        _clientsRepositoryMock.Setup(r => r.GetAsync()).ReturnsAsync(_fakeClients);
        var fakeClientDtos = _mapperMock.Object.Map<IEnumerable<ClientDto>>(_fakeClients);

        var testResult = Result.Ok(_fakeClients);
        var clientsService = new ClientsService(_mapperMock.Object, _clientsRepositoryMock.Object);

        // Act
        var clientsResult = await clientsService.GetClientsAsync();

        // Assert
        clientsResult.Should().BeEquivalentTo(testResult);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task GivenGetRequest_WhenRepositoryThrowException_ThenItShouldReturnServerErrorResultObject()
    {
        // Arrange
        _clientsRepositoryMock.Setup(r => r.GetAsync()).ThrowsAsync(new OutOfMemoryException());
        var clientsService = new ClientsService(_mapperMock.Object, _clientsRepositoryMock.Object);

        // Act
        var act = () => clientsService.GetClientsAsync();

        // Assert
        await act.Should().ThrowAsync<OutOfMemoryException>();
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task
        GivenExistingClient_WhenGetClientWithId_ThenItShouldReturnResultObjectWithTheClientDtoResult()
    {
        // Arrange
        var testClient = new Client { Id = 1, Name = "John Doe", Email = "john.doe@example.com" };
        var testClientDto = new ClientDto { Id = testClient.Id, Name = testClient.Name, Email = testClient.Email };
        _clientsRepositoryMock.Setup(r => r.GetAsync(testClient.Id)).ReturnsAsync(testClient);
        var clientsService = new ClientsService(_mapperMock.Object, _clientsRepositoryMock.Object);

        // Act
        var clientResult = await clientsService.GetClientByIdAsync(testClient.Id);

        // Assert
        clientResult.Should().BeEquivalentTo(Result.Ok(testClientDto));
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task GivenInvalidId_WhenGetClientWithId_ThenItShouldReturnBadRequestResultObjectWithErrorDetails()
    {
        // Arrange
        const int invalidId = -1;
        const int minId = 1;
        var clientServiceMock = new Mock<IClientsService>();
        var httpContext = new DefaultHttpContext();
        var controllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };
        var controller = new ClientsController(clientServiceMock.Object)
        {
            ControllerContext = controllerContext
        };

        // Act
        var result = await controller.Get(invalidId);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>()
            .Which.Value.Should().BeOfType<ErrorDetails>()
            .Which.Detail.Should().Contain(e =>
                e.Message == $"'{nameof(Client.Id)}' must be greater than or equal to '{minId}'.");
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task
        GivenNonExistentClient_WhenGetClientWithId_ThenItShouldReturnNofFoundResultObjectWithErrorDetails()
    {
        // Arrange
        const int nonExistentId = 100;
        var result = Result.Fail([$"Client with id '{nonExistentId}' not found."]);
        _clientsRepositoryMock.Setup(r => r.GetAsync(nonExistentId)).ReturnsAsync((Client)null!);
        var clientsService = new ClientsService(_mapperMock.Object, _clientsRepositoryMock.Object);
        // Act
        var clientResult = await clientsService.GetClientByIdAsync(nonExistentId);
        // Assert
        clientResult.Should().BeEquivalentTo(result);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task GivenGetWithIdRequest_WhenRepositoryThrowException_ThenItShouldReturnServerErrorResultObject()
    {
        // Arrange
        const int testClientId = 1;
        _clientsRepositoryMock.Setup(r => r.GetAsync(testClientId)).ThrowsAsync(new OutOfMemoryException());
        var clientsService = new ClientsService(_mapperMock.Object, _clientsRepositoryMock.Object);
        // Act
        var act = () => clientsService.GetClientByIdAsync(testClientId);
        // Assert
        await act.Should().ThrowAsync<OutOfMemoryException>();
    }
}