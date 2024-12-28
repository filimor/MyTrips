using System.Linq.Expressions;
using Bogus;
using FluentAssertions;
using FluentResults;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MyTrips.Application.Errors;
using MyTrips.Application.Interfaces;
using MyTrips.Application.Services;
using MyTrips.Application.Validators;
using MyTrips.Domain.Entities;
using MyTrips.Presentation.Controllers;
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
        var clientServiceMock = new Mock<IClientsService>();

        var httpContext = new DefaultHttpContext();
        var controllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };
        var controller = new ClientsController(clientServiceMock.Object, new ClientValidator())
        {
            ControllerContext = controllerContext
        };

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
        var clientServiceMock = new Mock<IClientsService>();

        var httpContext = new DefaultHttpContext();
        var controllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };
        var controller = new ClientsController(clientServiceMock.Object, new ClientValidator())
        {
            ControllerContext = controllerContext
        };

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
        var existingClient = new Faker<Client>()
            .RuleFor(c => c.Id, f => f.Random.Int())
            .RuleFor(c => c.Name, f => f.Name.FullName())
            .RuleFor(c => c.Email, f => _fixture.ClientStub.Email)
            .Generate();
        var mockClientList = new List<Client> { existingClient };
        _fixture.ClientsRepositoryMock.Setup(r => r.FindAsync(It.IsAny<Expression<Func<Client, bool>>>()))
            .ReturnsAsync((Expression<Func<Client, bool>> predicate) =>
                mockClientList.Where(predicate.Compile()));
        var clientsService = new ClientsService(_fixture.MapperMock.Object, _fixture.ClientsRepositoryMock.Object);
        var testResult =
            Result.Fail(new ConflictError(
                $"{nameof(Client)} with the {nameof(Client.Email)} '{existingClient.Email}' already exists."));

        // Act
        var clientResult = await clientsService.UpdateClientAsync(_fixture.UpdateClientDtoStub);

        // Assert
        clientResult.Should().BeEquivalentTo(testResult);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task GivenNonExistingClient_WhenTryToUpdateIt_ThenItShouldReturnFailObjectResultWithErrors()
    {
        const int nonExistentId = 100;
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
        var existingClient = new Faker<Client>()
            .RuleFor(c => c.Id, f => f.Random.Int())
            .RuleFor(c => c.Name, f => f.Name.FullName())
            .RuleFor(c => c.Email, f => _fixture.ClientStub.Email)
            .Generate();
        var mockClientList = new List<Client> { existingClient };
        _fixture.ClientsRepositoryMock.Setup(r => r.FindAsync(It.IsAny<Expression<Func<Client, bool>>>()))
            .ReturnsAsync((Expression<Func<Client, bool>> predicate) =>
                mockClientList.Where(predicate.Compile()));
        var clientsService = new ClientsService(_fixture.MapperMock.Object, _fixture.ClientsRepositoryMock.Object);
        var testResult =
            Result.Fail(new ConflictError(
                $"{nameof(Client)} with the {nameof(Client.Email)} '{existingClient.Email}' already exists."));

        // Act
        var clientResult = await clientsService.UpdateClientAsync(_fixture.UpdateClientDtoStub);

        // Assert
        clientResult.Should().BeEquivalentTo(testResult);
    }
}