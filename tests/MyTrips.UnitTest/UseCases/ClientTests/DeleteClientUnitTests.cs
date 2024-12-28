using FluentAssertions;
using FluentResults;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MyTrips.Application.Errors;
using MyTrips.Application.Interfaces;
using MyTrips.Application.Services;
using MyTrips.Domain.Entities;
using MyTrips.Presentation.Controllers;
using MyTrips.Presentation.Errors;
using MyTrips.UnitTest.Fixtures;

namespace MyTrips.UnitTest.UseCases.ClientTests;

[Collection("ClientsManagementUnit")]
public class DeleteClientUnitTests
{
    private readonly ClientsManagementFixture _fixture = new();

    [Fact]
    [Trait("Category", "Unit")]
    public async Task GivenExistingClient_WhenDeleteClient_ThenItShouldReturnOkResult()
    {
        // Arrange
        var clientsService = new ClientsService(_fixture.MapperMock.Object, _fixture.ClientsRepositoryMock.Object);

        // Act
        var requestResult = await clientsService.RemoveClientAsync(_fixture.ClientStub.Id);

        // Assert
        requestResult.Should().BeEquivalentTo(Result.Ok());
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task GivenInvalidId_WhenDeleteClient_ThenItShouldReturnFailResultWithValidationErrors()
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
        var response = await controller.Delete(invalidId);

        // Assert
        response.Should().BeOfType<BadRequestObjectResult>()
            .Which.Value.Should().BeOfType<BadRequestErrorDetails>()
            .Which.Errors.Should().Contain(e =>
                e == $"'{nameof(Client.Id)}' must be greater than or equal to '{minId}'.");
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task GivenNonExistingClient_WhenDeleteClient_ThenItShouldReturnFailResultWithNotFoundError()
    {
        // Arrange
        _fixture.ClientsRepositoryMock.Setup(r => r.DeleteAsync(_fixture.ClientStub.Id)).ReturnsAsync(0);
        var clientsService = new ClientsService(_fixture.MapperMock.Object, _fixture.ClientsRepositoryMock.Object);

        // Act
        var requestResult = await clientsService.RemoveClientAsync(_fixture.ClientStub.Id);

        // Assert
        requestResult.Should().BeEquivalentTo(Result.Fail(new NotFoundError(
            $"{nameof(Client)} with {nameof(Client.Id)} '{_fixture.ClientStub.Id}' not found.")));
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task GivenExistingClient_WhenDeleteClient_ThenItShouldPersistChanges()
    {
        // Arrange
        var clientsService = new ClientsService(_fixture.MapperMock.Object, _fixture.ClientsRepositoryMock.Object);

        // Act
        await clientsService.RemoveClientAsync(_fixture.ClientStub.Id);

        // Assert
        _fixture.ClientsRepositoryMock.Verify(r => r.DeleteAsync(_fixture.ClientStub.Id), Times.Once);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task GivenDeleteRequest_WhenRepositoryThrowException_ThenItShouldThrowTheException()
    {
        // Arrange
        const int testClientId = 1;
        _fixture.ClientsRepositoryMock.Setup(r => r.DeleteAsync(testClientId)).ThrowsAsync(new OutOfMemoryException());
        var clientsService = new ClientsService(_fixture.MapperMock.Object, _fixture.ClientsRepositoryMock.Object);

        // Act
        var act = async () => await clientsService.RemoveClientAsync(testClientId);

        // Assert
        await act.Should().ThrowAsync<OutOfMemoryException>();
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task
        GivenClientWithIdReferencedByForeignKey_WhenDeleteClient_ThenItShouldReturnFailResultWithConflictError()
    {
        // Arrange
        _fixture.ClientsRepositoryMock.Setup(r => r.DeleteAsync(_fixture.ClientStub.Id)).ReturnsAsync(-1);
        var clientsService = new ClientsService(_fixture.MapperMock.Object, _fixture.ClientsRepositoryMock.Object);

        // Act
        var requestResult = await clientsService.RemoveClientAsync(_fixture.ClientStub.Id);

        // Assert
        requestResult.Should().BeEquivalentTo(Result.Fail(new ConflictError(
            $"The {nameof(Client)} with {nameof(Client.Id)} '{_fixture.ClientStub.Id}' is referenced by another entity.")));
    }
}