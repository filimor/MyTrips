using MyTrips.Application.Errors;
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
        var testResult = Result.Ok(_fixture.ResponseClientDtoStub);

        // Act
        var clientResult = await _fixture.ClientsServiceStub.UpdateClientAsync(_fixture.UpdateClientDtoStub);

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
            .Which.Value.Should().BeOfType<BadRequestProblemDetails>();
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
            .Which.Value.Should().BeOfType<BadRequestProblemDetails>();
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task
        GivenAnExistingClient_WhenUpdateWithEmailOfOtherClient_ThenItShouldReturnFailResultObjectWithTheErrors()
    {
        // Arrange
        _fixture.OtherClientStub.Email = _fixture.UpdateClientDtoStub.Email;

        // Act
        var result = await _fixture.ClientsServiceStub.UpdateClientAsync(_fixture.UpdateClientDtoStub);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle().Which.Should().BeOfType<ConflictError>();
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task GivenNonExistingClient_WhenTryToUpdateIt_ThenItShouldReturnFailObjectResultWithErrors()
    {
        _fixture.UpdateClientDtoStub.Id = ClientsManagementFixture.NonExistentId;
        _fixture.ClientsRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Client>()))
            .ReturnsAsync((Client)null!);

        // Act
        var response = await _fixture.ClientsServiceStub.UpdateClientAsync(_fixture.UpdateClientDtoStub);

        // Assert
        response.IsFailed.Should().BeTrue();
        response.Errors.Should().ContainSingle().Which.Should().BeOfType<NotFoundError>();
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task GivenUpdateRequest_WhenRepositoryThrowException_ThenItShouldThrowTheException()
    {
        // Arrange
        _fixture.ClientsRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Client>()))
            .ThrowsAsync(new OutOfMemoryException());

        // Act
        var act = async () => await _fixture.ClientsServiceStub.UpdateClientAsync(_fixture.UpdateClientDtoStub);

        // Assert
        await act.Should().ThrowAsync<OutOfMemoryException>();
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task GivenAnExistingClient_WhenUpdateWithValidData_ThenItShouldPersistIt()
    {
        // Arrange & Act
        await _fixture.ClientsServiceStub.UpdateClientAsync(_fixture.UpdateClientDtoStub);

        // Assert
        _fixture.ClientsRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Client>()), Times.Once);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task GivenAnExistingClient_WhenTryToUpdateItWithEmailOfOtherClient_ThenItShouldNotPersistIt()
    {
        // Arrange
        _fixture.OtherClientStub.Email = _fixture.UpdateClientDtoStub.Email;

        // Act
        var result = await _fixture.ClientsServiceStub.UpdateClientAsync(_fixture.UpdateClientDtoStub);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle().Which.Should().BeOfType<ConflictError>();
    }
}