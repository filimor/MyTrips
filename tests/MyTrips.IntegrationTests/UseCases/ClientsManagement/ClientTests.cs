﻿using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using MyTrips.Application.Dtos;
using MyTrips.Domain.Entities;
using MyTrips.IntegrationTests.Extensions;
using MyTrips.IntegrationTests.Handlers;
using MyTrips.Presentation.Errors;
using Serilog;
using Xunit.Abstractions;

namespace MyTrips.IntegrationTests.UseCases.ClientsManagement;

public class ClientTests : IDisposable
{
    private readonly string _endpoint = "http://localhost:5068/api/clients";
    private readonly ILogger _output;
    private HttpClient _client;
    private WebApplicationFactory<Program> _factory;

    public ClientTests(ITestOutputHelper output)
    {
        _output = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .Enrich.WithExceptionData()
            .WriteTo.Console()
            .WriteTo.Debug()
            .WriteTo.TestOutput(output)
            .CreateLogger()
            .ForContext<ClientTests>();
    }

    public void Dispose()
    {
        _client.Dispose();
        _factory.Dispose();
        GC.SuppressFinalize(this);
    }

    private void SetDefaultFactory()
    {
        _factory = new WebApplicationFactory<Program>();
        _client = _factory.CreateDefaultClient(new LoggingHandler { InnerHandler = new HttpClientHandler() });
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task GivenClientsEndpoint_WhenRequestedGetClientWithoutId_ThenItShouldReturnOkWithHeadersAndContent()
    {
        // Arrange
        SetDefaultFactory();
        var request = new HttpRequestMessage(HttpMethod.Get, _endpoint);

        // Act
        var response = await _client.SendAsync(request);

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
        SetDefaultFactory();
        const int existingId = 1;
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_endpoint}/{existingId}");

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        var returnedClient = await response.DeserializedContentAsync<ClientDto>();
        response.EnsureSuccessStatusCode();
        response.Content.Headers.ContentType?.ToString().Should().Be("application/json; charset=utf-8");
        returnedClient.Should().BeOfType<ClientDto>();
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task GivenInvalidId_WhenRequestGetClientWithId_ThenItShouldReturnBadRequestWithHeadersAndContent()
    {
        // Arrange
        SetDefaultFactory();
        const int invalidId = -1;
        const int minId = 1;
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_endpoint}/{invalidId}");

        // Act
        var response = await _client.SendAsync(request);

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
        SetDefaultFactory();
        const int nonExistentId = int.MaxValue;
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_endpoint}/{nonExistentId}");

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        var errorDetails = await response.DeserializedContentAsync<ErrorDetails>();
        response.Should().HaveStatusCode(HttpStatusCode.NotFound);
        response.Content.Headers.ContentType?.ToString().Should().Be("application/problem+json; charset=utf-8");
        errorDetails!.Errors.Should().Contain($"Client with id '{nonExistentId}' not found.");
    }
}