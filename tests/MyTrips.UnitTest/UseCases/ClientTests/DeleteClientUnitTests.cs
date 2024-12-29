using MyTrips.Application.Errors;
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
        // Arrange & Act
        var requestResult = await _fixture.ClientsServiceStub.RemoveClientAsync(_fixture.ClientStub.Id);

        // Assert
        requestResult.Should().BeEquivalentTo(Result.Ok());
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task GivenInvalidId_WhenDeleteClient_ThenItShouldReturnFailResultWithValidationErrors()
    {
        // Arrange
        var controller = _fixture.NewClientsController();

        // Act
        var response = await controller.Delete(ClientsManagementFixture.InvalidId);

        // Assert
        response.Should().BeOfType<BadRequestObjectResult>()
            .Which.Value.Should().BeOfType<BadRequestProblemDetails>();
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task GivenNonExistingClient_WhenDeleteClient_ThenItShouldReturnFailResultWithNotFoundError()
    {
        // Arrange
        _fixture.ClientsRepositoryMock.Setup(r => r.DeleteAsync(_fixture.ClientStub.Id)).ReturnsAsync(0);

        // Act
        var result = await _fixture.ClientsServiceStub.RemoveClientAsync(_fixture.ClientStub.Id);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle().Which.Should().BeOfType<NotFoundError>();
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task GivenExistingClient_WhenDeleteClient_ThenItShouldPersistChanges()
    {
        // Arrange & Act
        await _fixture.ClientsServiceStub.RemoveClientAsync(_fixture.ClientStub.Id);

        // Assert
        _fixture.ClientsRepositoryMock.Verify(r => r.DeleteAsync(_fixture.ClientStub.Id), Times.Once);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task GivenDeleteRequest_WhenRepositoryThrowException_ThenItShouldThrowTheException()
    {
        // Arrange
        _fixture.ClientsRepositoryMock.Setup(r => r.DeleteAsync(ClientsManagementFixture.MinId))
            .ThrowsAsync(new OutOfMemoryException());

        // Act
        var act = async () => await _fixture.ClientsServiceStub.RemoveClientAsync(ClientsManagementFixture.MinId);

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

        // Act
        var result = await _fixture.ClientsServiceStub.RemoveClientAsync(_fixture.ClientStub.Id);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle().Which.Should().BeOfType<ConflictError>();
    }
}