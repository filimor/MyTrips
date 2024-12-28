using System.Net;
using FluentAssertions;
using MyTrips.Application.Dtos;
using MyTrips.Domain.Entities;
using MyTrips.IntegrationTests.Extensions;
using MyTrips.IntegrationTests.Fixtures;
using MyTrips.Presentation.Errors;

namespace MyTrips.IntegrationTests.UseCases.ClientsManagement;

[Collection("ClientsManagementIntegration")]
public class GetClientIntegrationTests
{
    [Fact]
    [Trait("Category", "Integration")]
    public async Task GivenClientsEndpoint_WhenRequestedGetClientWithoutId_ThenItShouldReturnOkWithHeadersAndContent()
    {
        // Arrange
        using var fixture = new ClientsManagementFixture();
        var request = new HttpRequestMessage(HttpMethod.Get, fixture.Endpoint);

        // Act
        var response = await fixture.DefaultHttpClient.SendAsync(request);

        // Assert
        var returnedClients = await response.DeserializedContentAsync<IEnumerable<Client>>();

        response.EnsureSuccessStatusCode();
        response.Should().HaveJsonContentType();
        returnedClients.Should().NotBeNull();
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task
        GivenExistingId_WhenRequestedGetClientWithId_ThenItShouldReturnOkWithHeadersAndContent()
    {
        // Arrange
        using var fixture = new ClientsManagementFixture();
        const int existingId = 1;
        var request = new HttpRequestMessage(HttpMethod.Get, $"{fixture.Endpoint}/{existingId}");

        // Act
        var response = await fixture.DefaultHttpClient.SendAsync(request);

        // Assert
        var returnedClient = await response.DeserializedContentAsync<ResponseClientDto>();
        response.EnsureSuccessStatusCode();
        response.Should().HaveJsonContentType();
        returnedClient.Should().BeOfType<ResponseClientDto>();
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task GivenInvalidId_WhenRequestGetClientWithId_ThenItShouldReturnBadRequestWithHeadersAndContent()
    {
        // Arrange
        using var fixture = new ClientsManagementFixture();
        const int invalidId = -1;
        const int minId = 1;
        var request = new HttpRequestMessage(HttpMethod.Get, $"{fixture.Endpoint}/{invalidId}");

        // Act
        var response = await fixture.DefaultHttpClient.SendAsync(request);

        // Assert
        var errorDetails = await response.DeserializedContentAsync<ErrorDetails>();
        response.Should().HaveStatusCode(HttpStatusCode.BadRequest);
        response.Should().HaveProblemContentType();
        errorDetails!.Errors.Should().Contain($"'{nameof(Client.Id)}' must be greater than or equal to '{minId}'.");
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task
        GivenNonExistentClient_WhenRequestGetClientWithId_ThenItShouldReturnNotFoundWithHeadersAndContent()
    {
        // Arrange
        using var fixture = new ClientsManagementFixture();
        const int nonExistentId = int.MaxValue;
        var request = new HttpRequestMessage(HttpMethod.Get, $"{fixture.Endpoint}/{nonExistentId}");

        // Act
        var response = await fixture.DefaultHttpClient.SendAsync(request);

        // Assert
        var errorDetails = await response.DeserializedContentAsync<ErrorDetails>();
        response.Should().HaveStatusCode(HttpStatusCode.NotFound);
        response.Should().HaveProblemContentType();
        errorDetails!.Errors.Should()
            .Contain($"{nameof(Client)} with {nameof(Client.Id)} '{nonExistentId}' not found.");
    }
}