﻿using System.Net;
using FluentAssertions;
using MyTrips.Application.Dtos;
using MyTrips.Domain.Entities;
using MyTrips.IntegrationTests.Extensions;
using MyTrips.IntegrationTests.Fixtures;
using MyTrips.Presentation.Errors;

namespace MyTrips.IntegrationTests.UseCases.ClientsManagement;

[Collection("ClientsManagement")]
public class GetClientIntegrationTests(ClientsManagementFixture fixture)
{
    [Fact]
    [Trait("Category", "Integration")]
    public async Task GivenClientsEndpoint_WhenRequestedGetClientWithoutId_ThenItShouldReturnOkWithHeadersAndContent()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Get, fixture.Endpoint);

        // Act
        var response = await fixture.Client.SendAsync(request);

        // Assert
        var returnedClients = await response.DeserializedContentAsync<IEnumerable<Client>>();

        response.EnsureSuccessStatusCode();
        response.Content.Headers.ContentType?.ToString().Should().Be("application/json; charset=utf-8");
        returnedClients.Should().NotBeNull();
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task
        GivenExistingId_WhenRequestedGetClientWithId_ThenItShouldReturnOkWithHeadersAndContent()
    {
        // Arrange
        const int existingId = 1;
        var request = new HttpRequestMessage(HttpMethod.Get, $"{fixture.Endpoint}/{existingId}");

        // Act
        var response = await fixture.Client.SendAsync(request);

        // Assert
        var returnedClient = await response.DeserializedContentAsync<ResponseClientDto>();
        response.EnsureSuccessStatusCode();
        response.Content.Headers.ContentType?.ToString().Should().Be("application/json; charset=utf-8");
        returnedClient.Should().BeOfType<ResponseClientDto>();
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task GivenInvalidId_WhenRequestGetClientWithId_ThenItShouldReturnBadRequestWithHeadersAndContent()
    {
        // Arrange
        const int invalidId = -1;
        const int minId = 1;
        var request = new HttpRequestMessage(HttpMethod.Get, $"{fixture.Endpoint}/{invalidId}");

        // Act
        var response = await fixture.Client.SendAsync(request);

        // Assert
        var errorDetails = await response.DeserializedContentAsync<ErrorDetails>();
        response.Should().HaveStatusCode(HttpStatusCode.BadRequest);
        response.Content.Headers.ContentType?.ToString().Should().Be("application/problem+json; charset=utf-8");
        errorDetails!.Errors.Should().Contain($"'{nameof(Client.Id)}' must be greater than or equal to '{minId}'.");
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task
        GivenNonExistentClient_WhenRequestGetClientWithId_ThenItShouldReturnNotFoundWithHeadersAndContent()
    {
        // Arrange
        const int nonExistentId = int.MaxValue;
        var request = new HttpRequestMessage(HttpMethod.Get, $"{fixture.Endpoint}/{nonExistentId}");

        // Act
        var response = await fixture.Client.SendAsync(request);

        // Assert
        var errorDetails = await response.DeserializedContentAsync<ErrorDetails>();
        response.Should().HaveStatusCode(HttpStatusCode.NotFound);
        response.Content.Headers.ContentType?.ToString().Should().Be("application/problem+json; charset=utf-8");
        errorDetails!.Errors.Should()
            .Contain($"{nameof(Client)} with {nameof(Client.Id)} '{nonExistentId}' not found.");
    }
}