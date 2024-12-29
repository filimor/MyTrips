using MyTrips.Application.Errors;
using MyTrips.Domain.Entities;
using MyTrips.Presentation.Errors;
using MyTrips.UnitTest.ClassData;
using MyTrips.UnitTest.Fixtures;

namespace MyTrips.UnitTest.UseCases.ClientTests;

[Collection("ClientsManagementUnit")]
public class CreateClientUnitTests
{
    private readonly ClientsManagementFixture _fixture = new();

    [Fact]
    [Trait("Category", "Unit")]
    public async Task GivenValidClient_WhenCreateClient_ThenItShouldReturnOkResultObjectWithTheDto()
    {
        // Arrange
        var testResult = Result.Ok(_fixture.ResponseClientDtoStub);

        // Act
        var clientResult = await _fixture.ClientsServiceStub.AddNewClientAsync(_fixture.CreateClientDtoStub);

        // Assert
        clientResult.Should().BeEquivalentTo(testResult);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task GivenValidClient_WhenCreateClient_ThenItShouldBePersisted()
    {
        // Arrange & Act
        await _fixture.ClientsServiceStub.AddNewClientAsync(_fixture.CreateClientDtoStub);

        // Assert
        _fixture.ClientsRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Client>()), Times.Once);
    }

    [Theory]
    [ClassData(typeof(InvalidStringClassData))]
    [Trait("Category", "Unit")]
    public async Task GivenInvalidName_WhenTryToCreateClient_ThenItShouldReturnBadRequestResponseWithTheError(
        string name)
    {
        // Arrange
        _fixture.CreateClientDtoStub.Name = name;
        var controller = _fixture.NewClientsController();

        // Act
        var response = await controller.Post(_fixture.CreateClientDtoStub);

        // Assert
        response.Should().BeOfType<BadRequestObjectResult>()
            .Which.Value.Should().BeOfType<BadRequestProblemDetails>();
    }

    [Theory]
    [ClassData(typeof(InvalidEmailClassData))]
    [Trait("Category", "Unit")]
    public async Task GivenInvalidEmail_WhenTryToCreateClient_ThenItShouldReturnBadRequestResponseWithTheError(
        string email)
    {
        // Arrange
        _fixture.CreateClientDtoStub.Email = email;
        var controller = _fixture.NewClientsController();

        // Act
        var response = await controller.Post(_fixture.CreateClientDtoStub);

        // Assert
        response.Should().BeOfType<BadRequestObjectResult>()
            .Which.Value.Should().BeOfType<BadRequestProblemDetails>();
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task GivenExistingEmail_WhenTryToCreateClient_ThenItShouldReturnFailResultObjectWithTheErrors()
    {
        // Arrange
        _fixture.OtherClientStub.Email = _fixture.UpdateClientDtoStub.Email;

        // Act
        var result = await _fixture.ClientsServiceStub.AddNewClientAsync(_fixture.CreateClientDtoStub);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle().Which.Should().BeOfType<ConflictError>();
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task GivenExistingEmail_WhenTryToCreateClient_ThenItShouldReturnNotPersistIt()
    {
        // Arrange
        _fixture.OtherClientStub.Email = _fixture.UpdateClientDtoStub.Email;

        // Act
        await _fixture.ClientsServiceStub.AddNewClientAsync(_fixture.CreateClientDtoStub);

        // Assert
        _fixture.ClientsRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Client>()), Times.Never);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task GivenCreateRequest_WhenRepositoryThrowException_ThenItShouldThrowTheException()
    {
        // Arrange
        _fixture.ClientsRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Client>()))
            .ThrowsAsync(new OutOfMemoryException());

        // Act
        var act = async () => await _fixture.ClientsServiceStub.AddNewClientAsync(_fixture.CreateClientDtoStub);

        // Assert
        await act.Should().ThrowAsync<OutOfMemoryException>();
    }
}