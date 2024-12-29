using MyTrips.Application.Errors;
using MyTrips.Application.Services;
using MyTrips.Domain.Entities;
using MyTrips.Presentation.Errors;
using MyTrips.UnitTest.ClassData;
using MyTrips.UnitTest.Fixtures;

namespace MyTrips.UnitTest.UseCases.ClientTests;

[Collection("ClientsManagementUnit")]
public class UpdateClientUnitTests
{
    private readonly ClientsManagementFixture _fixture = new();

    [Fact]
    [Trait("Category", "Unit")]
    public async Task GivenAnExistingClient_WhenUpdateWithValidData_ThenItShouldReturnOkResultObjectWithUpdatedDto()
    {
        // Arrange
        var clientsService = new ClientsService(_fixture.MapperMock.Object, _fixture.ClientsRepositoryMock.Object);
        var testResult = Result.Ok(_fixture.ResponseClientDtoStub);

        // Act
        var clientResult = await clientsService.UpdateClientAsync(_fixture.UpdateClientDtoStub);

        // Assert
        clientResult.Should().BeEquivalentTo(testResult);
    }

    [Theory]
    [Trait("Category", "Unit")]
    [ClassData(typeof(InvalidStringClassData))]
    public async Task GivenAnExistingClient_WhenUpdateWithInvalidName_ThenItShouldReturnFailResultObjectWithTheErrors(
        string name)
    {
        // Arrange
        _fixture.UpdateClientDtoStub.Name = name;
        var controller = _fixture.NewClientsController();

        // Act
        var response = await controller.Put(_fixture.UpdateClientDtoStub);

        // Assert
        response.Should().BeOfType<BadRequestObjectResult>()
            .Which.Value.Should().BeOfType<BadRequestErrorDetails>()
            .Which.Errors.Should().ContainMatch($"*{nameof(Client.Name)}*");
    }


    [Theory]
    [Trait("Category", "Unit")]
    [ClassData(typeof(InvalidEmailClassData))]
    public async Task GivenAnExistingClient_WhenUpdateWithInvalidEmail_ThenItShouldReturnFailResultObjectWithTheErrors(
        string email)
    {
        // Arrange
        _fixture.UpdateClientDtoStub.Email = email;
        var controller = _fixture.NewClientsController();

        // Act
        var response = await controller.Put(_fixture.UpdateClientDtoStub);

        // Assert
        response.Should().BeOfType<BadRequestObjectResult>()
            .Which.Value.Should().BeOfType<BadRequestErrorDetails>()
            .Which.Errors.Should().ContainMatch($"*{nameof(Client.Email)}*");
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task
        GivenAnExistingClient_WhenUpdateWithEmailOfOtherClient_ThenItShouldReturnFailResultObjectWithTheErrors()
    {
        // Arrange
        _fixture.OtherClientStub.Email = _fixture.UpdateClientDtoStub.Email;
        var clientsService = new ClientsService(_fixture.MapperMock.Object, _fixture.ClientsRepositoryMock.Object);
        var testResult =
            Result.Fail(new ConflictError(
                $"{nameof(Client)} with the {nameof(Client.Email)} '{_fixture.ClientStub.Email}' already exists."));

        // Act
        var clientResult = await clientsService.UpdateClientAsync(_fixture.UpdateClientDtoStub);

        // Assert
        clientResult.Should().BeEquivalentTo(testResult);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task GivenNonExistingClient_WhenTryToUpdateIt_ThenItShouldReturnFailObjectResultWithErrors()
    {
        const int nonExistentId = int.MaxValue;
        _fixture.UpdateClientDtoStub.Id = nonExistentId;

        var result =
            Result.Fail(new NotFoundError($"{nameof(Client)} with {nameof(Client.Id)} '{nonExistentId}' not found."));
        _fixture.ClientsRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Client>()))
            .ReturnsAsync((Client)null!);

        var clientsService = new ClientsService(_fixture.MapperMock.Object, _fixture.ClientsRepositoryMock.Object);

        // Act
        var response = await clientsService.UpdateClientAsync(_fixture.UpdateClientDtoStub);

        // Assert
        response.Should().BeEquivalentTo(result);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task GivenUpdateRequest_WhenRepositoryThrowException_ThenItShouldThrowTheException()
    {
        // Arrange
        _fixture.ClientsRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Client>()))
            .ThrowsAsync(new OutOfMemoryException());
        var clientsService = new ClientsService(_fixture.MapperMock.Object, _fixture.ClientsRepositoryMock.Object);

        // Act
        var act = async () => await clientsService.UpdateClientAsync(_fixture.UpdateClientDtoStub);

        // Assert
        await act.Should().ThrowAsync<OutOfMemoryException>();
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task GivenAnExistingClient_WhenUpdateWithValidData_ThenItShouldPersistIt()
    {
        // Arrange
        var clientsService = new ClientsService(_fixture.MapperMock.Object, _fixture.ClientsRepositoryMock.Object);

        // Act
        await clientsService.UpdateClientAsync(_fixture.UpdateClientDtoStub);

        // Assert
        _fixture.ClientsRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Client>()), Times.Once);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task GivenAnExistingClient_WhenTryToUpdateItWithEmailOfOtherClient_ThenItShouldNotPersistIt()
    {
        // Arrange
        _fixture.OtherClientStub.Email = _fixture.UpdateClientDtoStub.Email;
        var clientsService = new ClientsService(_fixture.MapperMock.Object, _fixture.ClientsRepositoryMock.Object);
        var testResult =
            Result.Fail(new ConflictError(
                $"{nameof(Client)} with the {nameof(Client.Email)} '{_fixture.ClientStub.Email}' already exists."));

        // Act
        var clientResult = await clientsService.UpdateClientAsync(_fixture.UpdateClientDtoStub);

        // Assert
        clientResult.Should().BeEquivalentTo(testResult);
    }
}