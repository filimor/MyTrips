using FluentAssertions;
using FluentResults;
using Moq;
using MyTrips.Application.Services;
using MyTrips.UnitTest.Fixtures;

namespace MyTrips.UnitTest.UseCases.ClientTests;

[Collection("ClientsManagementUnit")]
public class DeleteClientUnitTests(ClientsManagementFixture fixture)
{
    [Fact]
    [Trait("Category", "Unit")]
    public async Task GivenExistingClient_WhenDeleteClient_ThenItShouldReturnOkResult()
    {
        // Arrange
        fixture.ClientsRepositoryMock.Setup(r => r.GetAsync(fixture.ClientStub.Id)).ReturnsAsync(fixture.ClientStub);
        var clientsService = new ClientsService(fixture.MapperMock.Object, fixture.ClientsRepositoryMock.Object);

        // Act
        var requestResult = await clientsService.RemoveClientAsync(fixture.ClientStub.Id);

        // Assert
        requestResult.Should().BeEquivalentTo(Result.Ok());
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task GivenInvalidId_WhenDeleteClient_ThenItShouldReturnFailResultWithValidationErrors()
    {
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task GivenNonExistingClient_WhenDeleteClient_ThenItShouldReturnFailResultWithNotFoundError()
    {
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task GivenExistingClient_WhenDeleteClient_ThenItShouldPersistChanges()
    {
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task GivenNonExistingClient_WhenDeleteClient_ThenItShouldNotPersistAnyData()
    {
    }
}