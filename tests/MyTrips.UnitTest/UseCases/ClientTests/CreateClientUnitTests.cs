using MyTrips.Application.Errors;
using MyTrips.Application.Services;
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
        var clientsService = new ClientsService(_fixture.MapperMock.Object, _fixture.ClientsRepositoryMock.Object);
        var testResult = Result.Ok(_fixture.ResponseClientDtoStub);

        // Act
        var clientResult = await clientsService.AddNewClientAsync(_fixture.CreateClientDtoStub);

        // Assert
        clientResult.Should().BeEquivalentTo(testResult);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task GivenValidClient_WhenCreateClient_ThenItShouldBePersisted()
    {
        // Arrange
        var clientsService = new ClientsService(_fixture.MapperMock.Object, _fixture.ClientsRepositoryMock.Object);

        // Act
        await clientsService.AddNewClientAsync(_fixture.CreateClientDtoStub);

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
            .Which.Value.Should().BeOfType<BadRequestErrorDetails>()
            .Which.Errors.Should().ContainMatch($"*{nameof(Client.Name)}*");
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
            .Which.Value.Should().BeOfType<BadRequestErrorDetails>()
            .Which.Errors.Should().ContainMatch($"*{nameof(Client.Email)}*");
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task GivenExistingEmail_WhenTryToCreateClient_ThenItShouldReturnFailResultObjectWithTheErrors()
    {
        // Arrange
        _fixture.OtherClientStub.Email = _fixture.UpdateClientDtoStub.Email;
        var clientsService = new ClientsService(_fixture.MapperMock.Object, _fixture.ClientsRepositoryMock.Object);
        var testResult =
            Result.Fail(new ConflictError(
                $"{nameof(Client)} with the {nameof(Client.Email)} '{_fixture.ClientStub.Email}' already exists."));

        // Act
        var clientResult = await clientsService.AddNewClientAsync(_fixture.CreateClientDtoStub);

        // Assert
        clientResult.Should().BeEquivalentTo(testResult);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task GivenExistingEmail_WhenTryToCreateClient_ThenItShouldReturnNotPersistIt()
    {
        // Arrange
        _fixture.OtherClientStub.Email = _fixture.UpdateClientDtoStub.Email;
        var clientsService = new ClientsService(_fixture.MapperMock.Object, _fixture.ClientsRepositoryMock.Object);

        // Act
        await clientsService.AddNewClientAsync(_fixture.CreateClientDtoStub);

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
        var clientsService = new ClientsService(_fixture.MapperMock.Object, _fixture.ClientsRepositoryMock.Object);

        // Act
        var act = async () => await clientsService.AddNewClientAsync(_fixture.CreateClientDtoStub);

        // Assert
        await act.Should().ThrowAsync<OutOfMemoryException>();
    }
}