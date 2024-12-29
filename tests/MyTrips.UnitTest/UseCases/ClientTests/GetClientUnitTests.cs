using MyTrips.Application.Dtos;
using MyTrips.Application.Errors;
using MyTrips.Domain.Entities;
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
        var fakeClientDtos =
            _fixture.MapperMock.Object.Map<IEnumerable<ResponseClientDto>>(_fixture.ClientsCollectionStub);
        var testResult = Result.Ok(fakeClientDtos);

        // Act
        var clientsResult = await _fixture.ClientsServiceStub.GetAllClientsAsync();

        // Assert
        clientsResult.Should().BeEquivalentTo(testResult);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task GivenGetRequest_WhenRepositoryThrowException_ThenItShouldThrowTheException()
    {
        // Arrange
        _fixture.ClientsRepositoryMock.Setup(r => r.GetAsync()).ThrowsAsync(new OutOfMemoryException());

        // Act
        var act = async () => await _fixture.ClientsServiceStub.GetAllClientsAsync();

        // Assert
        await act.Should().ThrowAsync<OutOfMemoryException>();
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task
        GivenExistingClient_WhenGetClientWithId_ThenItShouldReturnOkResultObjectWithTheClientDtoResult()
    {
        // Arrange & Act
        var clientResult = await _fixture.ClientsServiceStub.GetClientByIdAsync(_fixture.ClientStub.Id);

        // Assert
        clientResult.Should().BeEquivalentTo(Result.Ok(_fixture.ResponseClientDtoStub));
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task
        GivenInvalidId_WhenGetClientWithIdRequest_ThenItShouldReturnBadRequestResponseWithErrorDetails()
    {
        // Arrange
        var controller = _fixture.NewClientsController();

        // Act
        var response = await controller.Get(ClientsManagementFixture.InvalidId);

        // Assert
        response.Should().BeOfType<BadRequestObjectResult>()
            .Which.Value.Should().BeOfType<BadRequestErrorDetails>()
            .Which.Errors.Should().ContainMatch($"*{nameof(Client.Id)}*");
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task
        GivenNonExistentClient_WhenTryGetClient_ThenItShouldReturnNotFoundResultObjectWithErrorDetails()
    {
        // Arrange
        _fixture.ClientsRepositoryMock.Setup(r => r.GetAsync(ClientsManagementFixture.NonExistentId))
            .ReturnsAsync((Client)null!);

        // Act
        var result = await _fixture.ClientsServiceStub.GetClientByIdAsync(ClientsManagementFixture.NonExistentId);

        //Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle().Which.Should().BeOfType<NotFoundError>();
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task GivenCreateRequest_WhenRepositoryThrowException_ThenItShouldThrowTheException()
    {
        // Arrange
        _fixture.ClientsRepositoryMock.Setup(r => r.GetAsync(ClientsManagementFixture.MinId))
            .ThrowsAsync(new OutOfMemoryException());

        // Act
        var act = async () => await _fixture.ClientsServiceStub.GetClientByIdAsync(ClientsManagementFixture.MinId);

        // Assert
        await act.Should().ThrowAsync<OutOfMemoryException>();
    }
}