using FluentAssertions;
using FluentResults;
using Moq;
using MyTrips.Application.Services;
using MyTrips.Domain.Entities;
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

    [Fact]
    [Trait("Category", "Unit")]
    public async Task GivenAnExistingClient_WhenUpdateWithInvalidName_ThenItShouldReturnFailResultObjectWithTheErrors()
    {
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task GivenAnExistingClient_WhenUpdateWithInvalidEmail_ThenItShouldReturnFailResultObjectWithTheErrors()
    {
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task
        GivenAnExistingClient_WhenUpdateWithEmailOfOtherClient_ThenItShouldReturnFailResultObjectWithTheErrors()
    {
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task GivenNonExistingClient_WhenTryToUpdateIt_ThenItShouldReturnFailObjectResultWithErrors()
    {
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task GivenUpdateRequest_WhenRepositoryThrowException_ThenItShouldThrowTheException()
    {
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task GivenAnExistingClient_WhenUpdateWithValidData_ThenItShouldNotPersistIt()
    {
    }


    [Fact]
    [Trait("Category", "Unit")]
    public async Task GivenAnExistingClient_WhenTryToUpdateItWithInvalidName_ThenItShouldNotPersistIt()
    {
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task GivenAnExistingClient_WhenTryToUpdateItWithInvalidEmail_ThenItShouldNotPersistIt()
    {
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task GivenAnExistingClient_WhenTryToUpdateItWithEmailOfOtherClient_ThenItShouldNotPersistIt()
    {
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task GivenNonExistingClient_WhenTryToUpdateIt_ThenItShouldNotPersistIt()
    {
    }
}