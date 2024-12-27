﻿using FluentAssertions;
using FluentResults;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
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
public class UpdateClientUnitTests(ClientsManagementFixture fixture)
{
    [Fact]
    [Trait("Category", "Unit")]
    public async Task GivenAnExistingClient_WhenUpdateWithValidData_ThenItShouldReturnOkResultObjectWithUpdatedDto()
    {
        // Arrange
        fixture.ClientsRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Client>()))
            .ReturnsAsync((Client client) => client);
        var clientsService = new ClientsService(fixture.MapperMock.Object, fixture.ClientsRepositoryMock.Object);
        var testResult = Result.Ok(fixture.ResponseClientDtoStub);

        // Act
        var clientResult = await clientsService.UpdateClientAsync(fixture.UpdateClientDtoStub);

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
        fixture.UpdateClientDtoStub.Name = name;
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
        var response = await controller.Put(fixture.UpdateClientDtoStub);

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
        fixture.UpdateClientDtoStub.Email = email;
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
        var response = await controller.Put(fixture.UpdateClientDtoStub);

        // Assert
        response.Should().BeOfType<BadRequestObjectResult>()
            .Which.Value.Should().BeOfType<BadRequestErrorDetails>()
            .Which.Errors.Should().ContainMatch($"*{nameof(Client.Email)}*");
    }

    //[Fact]
    //[Trait("Category", "Unit")]
    //public async Task
    //    GivenAnExistingClient_WhenUpdateWithEmailOfOtherClient_ThenItShouldReturnFailResultObjectWithTheErrors()
    //{
    //}

    //[Fact]
    //[Trait("Category", "Unit")]
    //public async Task GivenNonExistingClient_WhenTryToUpdateIt_ThenItShouldReturnFailObjectResultWithErrors()
    //{
    //}

    //[Fact]
    //[Trait("Category", "Unit")]
    //public async Task GivenUpdateRequest_WhenRepositoryThrowException_ThenItShouldThrowTheException()
    //{
    //}

    //[Fact]
    //[Trait("Category", "Unit")]
    //public async Task GivenAnExistingClient_WhenUpdateWithValidData_ThenItShouldNotPersistIt()
    //{
    //}


    //[Theory]
    //[ClassData(typeof(InvalidStringClassData))]
    //[Trait("Category", "Unit")]
    //public async Task GivenAnExistingClient_WhenTryToUpdateItWithInvalidName_ThenItShouldNotPersistIt(string name)
    //{
    //}

    //[Theory]
    //[ClassData(typeof(InvalidEmailClassData))]
    //[Trait("Category", "Unit")]
    //public async Task GivenAnExistingClient_WhenTryToUpdateItWithInvalidEmail_ThenItShouldNotPersistIt(string email)
    //{
    //}

    //[Fact]
    //[Trait("Category", "Unit")]
    //public async Task GivenAnExistingClient_WhenTryToUpdateItWithEmailOfOtherClient_ThenItShouldNotPersistIt()
    //{
    //}

    //[Fact]
    //[Trait("Category", "Unit")]
    //public async Task GivenNonExistingClient_WhenTryToUpdateIt_ThenItShouldNotPersistIt()
    //{
    //}
}