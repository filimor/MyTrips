using System.Globalization;
using AutoMapper;
using Bogus;
using FluentAssertions;
using FluentResults;
using FluentValidation;
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

namespace MyTrips.UnitTest.UseCases.ClientTests;

public class GetClientUnitTests
{
    private readonly Mock<IClientsRepository> _clientsRepositoryMock = new();

    private readonly IEnumerable<Client> _fakeClients = new Faker<Client>()
        .RuleFor(c => c.Name, f => f.Name.FullName())
        .RuleFor(c => c.Email, f => f.Internet.Email())
        .Generate(10);

    private readonly Mock<IMapper> _mapperMock = new();

    public GetClientUnitTests()
    {
        CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
        CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;

        _mapperMock.Setup(m => m.Map<ResponseClientDto>(It.IsAny<Client>()))
            .Returns((Client client) => new ResponseClientDto
            {
                Id = client.Id,
                Name = client.Name,
                Email = client.Email
            });
        _mapperMock.Setup(m => m.Map<IEnumerable<ResponseClientDto>>(It.IsAny<IEnumerable<Client>>()))
            .Returns((IEnumerable<Client> clients) => clients.Select(c => new ResponseClientDto
            {
                Id = c.Id,
                Name = c.Name,
                Email = c.Email
            }));
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task GivenExistingClients_WhenGetClients_ThenItShouldReturnOkResultObjectWithTheDtos()
    {
        // Arrange
        _clientsRepositoryMock.Setup(r => r.GetAsync()).ReturnsAsync(_fakeClients);
        var fakeClientDtos = _mapperMock.Object.Map<IEnumerable<ResponseClientDto>>(_fakeClients);
        var testResult = Result.Ok(fakeClientDtos);
        var clientsService = new ClientsService(_mapperMock.Object, _clientsRepositoryMock.Object);

        // Act
        var clientsResult = await clientsService.GetClientsAsync();

        // Assert
        clientsResult.Should().BeEquivalentTo(testResult);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task GivenGetRequest_WhenRepositoryThrowException_ThenItShouldThrowTheException()
    {
        // Arrange
        _clientsRepositoryMock.Setup(r => r.GetAsync()).ThrowsAsync(new OutOfMemoryException());
        var clientsService = new ClientsService(_mapperMock.Object, _clientsRepositoryMock.Object);

        // Act
        var act = async () => await clientsService.GetClientsAsync();

        // Assert
        await act.Should().ThrowAsync<OutOfMemoryException>();
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task
        GivenExistingClient_WhenGetClientWithId_ThenItShouldReturnOkResultObjectWithTheClientDtoResult()
    {
        // Arrange
        var testClient = new Client(1, "John Doe", "john.doe@example.com");
        var testClientDto = new ResponseClientDto
            { Id = testClient.Id, Name = testClient.Name, Email = testClient.Email };
        _clientsRepositoryMock.Setup(r => r.GetAsync(testClient.Id)).ReturnsAsync(testClient);
        var clientsService = new ClientsService(_mapperMock.Object, _clientsRepositoryMock.Object);

        // Act
        var clientResult = await clientsService.GetClientByIdAsync(testClient.Id);

        // Assert
        clientResult.Should().BeEquivalentTo(Result.Ok(testClientDto));
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task
        GivenInvalidId_WhenGetClientWithIdRequest_ThenItShouldReturnBadRequestResponseWithErrorDetails()
    {
        // Arrange
        const int invalidId = -1;
        const int minId = 1;
        var clientServiceMock = new Mock<IClientsService>();
        var validatorMock = new Mock<IValidator<Client>>();
        var httpContext = new DefaultHttpContext();
        var controllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };
        var controller = new ClientsController(clientServiceMock.Object, validatorMock.Object)
        {
            ControllerContext = controllerContext
        };

        // Act
        var response = await controller.Get(invalidId);

        // Assert
        response.Should().BeOfType<BadRequestObjectResult>()
            .Which.Value.Should().BeOfType<BadRequestErrorDetails>()
            .Which.Errors.Should().Contain(e =>
                e == $"'{nameof(Client.Id)}' must be greater than or equal to '{minId}'.");
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task
        GivenNonExistentClient_WhenTryGetClient_ThenItShouldReturnNotFoundResultObjectWithErrorDetails()
    {
        // Arrange
        const int nonExistentId = 100;
        var result = Result.Fail([$"{nameof(Client)} with {nameof(Client.Id)} '{nonExistentId}' not found."]);
        _clientsRepositoryMock.Setup(r => r.GetAsync(nonExistentId)).ReturnsAsync((Client)null!);
        var clientsService = new ClientsService(_mapperMock.Object, _clientsRepositoryMock.Object);
        // Act
        var response = await clientsService.GetClientByIdAsync(nonExistentId);
        // Assert
        response.Should().BeEquivalentTo(result);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task GivenCreateRequest_WhenRepositoryThrowException_ThenItShouldThrowTheException()
    {
        // Arrange
        const int testClientId = 1;
        _clientsRepositoryMock.Setup(r => r.GetAsync(testClientId)).ThrowsAsync(new OutOfMemoryException());
        var clientsService = new ClientsService(_mapperMock.Object, _clientsRepositoryMock.Object);
        // Act
        var act = async () => await clientsService.GetClientByIdAsync(testClientId);
        // Assert
        await act.Should().ThrowAsync<OutOfMemoryException>();
    }
}