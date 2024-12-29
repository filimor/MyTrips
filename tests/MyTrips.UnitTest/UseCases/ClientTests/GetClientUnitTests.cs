using FluentValidation;
using MyTrips.Application.Dtos;
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
        _fixture.ClientsRepositoryMock.Setup(r => r.GetAsync()).ReturnsAsync(_fixture.ClientsCollectionStub);
        var fakeClientDtos =
            _fixture.MapperMock.Object.Map<IEnumerable<ResponseClientDto>>(_fixture.ClientsCollectionStub);
        var testResult = Result.Ok(fakeClientDtos);


        // Act
        var clientsResult = await _fixture.ClientsServiceStub.GetClientsAsync();

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
        var act = async () => await _fixture.ClientsServiceStub.GetClientsAsync();

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


        // Act
        var clientResult = await _fixture.ClientsServiceStub.GetClientByIdAsync(testClient.Id);

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
        var validatorMock = new Mock<IValidator<Client>>();
        var controller = _fixture.NewClientsController(validatorMock.Object);
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
        _fixture.ClientsRepositoryMock.Setup(r => r.GetAsync(ClientsManagementFixture.NonExistentId))
            .ReturnsAsync((Client)null!);

        // Act
        var response = await _fixture.ClientsServiceStub.GetClientByIdAsync(ClientsManagementFixture.NonExistentId);

        // Assert
        //response.Should().BeEquivalentTo(result);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task GivenCreateRequest_WhenRepositoryThrowException_ThenItShouldThrowTheException()
    {
        // Arrange
        const int testClientId = 1;
        _fixture.ClientsRepositoryMock.Setup(r => r.GetAsync(testClientId)).ThrowsAsync(new OutOfMemoryException());


        // Act
        var act = async () => await _fixture.ClientsServiceStub.GetClientByIdAsync(testClientId);

        // Assert
        await act.Should().ThrowAsync<OutOfMemoryException>();
    }
}