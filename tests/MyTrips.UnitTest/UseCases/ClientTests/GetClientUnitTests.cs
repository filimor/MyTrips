// Arrange

using FluentAssertions;
using FluentResults;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MyTrips.Application.Dtos;
using MyTrips.Application.Errors;
using MyTrips.Application.Interfaces;
using MyTrips.Application.Services;
using MyTrips.Domain.Entities;
using MyTrips.Presentation.Controllers;
using MyTrips.Presentation.Errors;
using MyTrips.UnitTest.Fixtures;

namespace MyTrips.UnitTest.UseCases.ClientTests;

[Collection("ClientsManagementUnit")]
public class GetClientUnitTests
{
    private readonly ClientsManagementFixture _fixture = new();

    [Fact]
    [Trait("Category", "Unit")]
    public async Task GivenExistingClients_WhenGetClients_ThenItShouldReturnOkResultObjectWithTheDtos()
    {
        // Arrange
        _fixture.ClientsRepositoryMock.Setup(r => r.GetAsync()).ReturnsAsync(_fixture.ClientsCollectionStub);
        var fakeClientDtos =
            _fixture.MapperMock.Object.Map<IEnumerable<ResponseClientDto>>(_fixture.ClientsCollectionStub);
        var testResult = Result.Ok(fakeClientDtos);
        var clientsService = new ClientsService(_fixture.MapperMock.Object, _fixture.ClientsRepositoryMock.Object);

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
        _fixture.ClientsRepositoryMock.Setup(r => r.GetAsync()).ThrowsAsync(new OutOfMemoryException());
        var clientsService = new ClientsService(_fixture.MapperMock.Object, _fixture.ClientsRepositoryMock.Object);

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
        _fixture.ClientsRepositoryMock.Setup(r => r.GetAsync(testClient.Id)).ReturnsAsync(testClient);
        var clientsService = new ClientsService(_fixture.MapperMock.Object, _fixture.ClientsRepositoryMock.Object);

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
        var result =
            Result.Fail(new NotFoundError($"{nameof(Client)} with {nameof(Client.Id)} '{nonExistentId}' not found."));
        _fixture.ClientsRepositoryMock.Setup(r => r.GetAsync(nonExistentId)).ReturnsAsync((Client)null!);
        var clientsService = new ClientsService(_fixture.MapperMock.Object, _fixture.ClientsRepositoryMock.Object);

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
        _fixture.ClientsRepositoryMock.Setup(r => r.GetAsync(testClientId)).ThrowsAsync(new OutOfMemoryException());
        var clientsService = new ClientsService(_fixture.MapperMock.Object, _fixture.ClientsRepositoryMock.Object);

        // Act
        var act = async () => await clientsService.GetClientByIdAsync(testClientId);

        // Assert
        await act.Should().ThrowAsync<OutOfMemoryException>();
    }
}