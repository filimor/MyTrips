using System.Net;
using System.Text;
using FluentAssertions;
using MyTrips.Application.Dtos;
using MyTrips.Domain.Entities;
using MyTrips.IntegrationTests.Extensions;
using MyTrips.IntegrationTests.Fixtures;
using MyTrips.Presentation.Errors;
using MyTrips.UnitTest.ClassData;
using Newtonsoft.Json;

namespace MyTrips.IntegrationTests.UseCases.ClientsManagement;

[Collection("ClientsManagementIntegration")]
public class UpdateClientIntegrationTests(ClientsManagementFixture fixture)
{
    [Fact]
    [Trait("Category", "Integration")]
    public async Task GivenValidClientDto_WhenRequestedUpdateClient_ThenItShouldReturnSuccessWithHeadersAndContent()
    {
        // Arrange
        var json = JsonConvert.SerializeObject(fixture.UpdateClientDtoStub);
        StringContent data = new(json, Encoding.UTF8, "application/json");
        var request = new HttpRequestMessage(HttpMethod.Put, fixture.Endpoint)
        {
            Content = data
        };

        // Act
        var response = await fixture.DefaultHttpClient.SendAsync(request);

        // Assert
        var returnedClient = await response.DeserializedContentAsync<ResponseClientDto>();
        response.Should().HaveStatusCode(HttpStatusCode.OK);
        response.Should().HaveJsonContentType();
        returnedClient.Should().BeEquivalentTo(fixture.ResponseClientDtoStub);
    }

    [Theory]
    [Trait("Category", "Integration")]
    [ClassData(typeof(InvalidStringClassData))]
    public async Task
        GivenClientDtoWithInvalidName_WhenRequestedUpdateClient_ThenItShouldReturnBadRequestWithHeadersAndContent(
            string name)
    {
        // Arrange
        fixture.UpdateClientDtoStub.Name = name;
        var json = JsonConvert.SerializeObject(fixture.UpdateClientDtoStub);
        StringContent data = new(json, Encoding.UTF8, "application/json");
        var request = new HttpRequestMessage(HttpMethod.Put, fixture.Endpoint)
        {
            Content = data
        };

        // Act
        var response = await fixture.DefaultHttpClient.SendAsync(request);

        // Assert
        var errorDetails = await response.DeserializedContentAsync<ErrorDetails>();
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        response.Should().HaveProblemContentType();
        errorDetails!.Errors.Should().ContainMatch($"*{nameof(Client.Name)}*");
    }

    [Theory]
    [Trait("Category", "Integration")]
    [ClassData(typeof(InvalidEmailClassData))]
    public async Task
        GivenClientDtoWithInvalidEmail_WhenRequestedUpdateClient_ThenItShouldReturnBadRequestWithHeadersAndContent(
            string email)
    {
        // Arrange
        fixture.UpdateClientDtoStub.Email = email;
        var json = JsonConvert.SerializeObject(fixture.UpdateClientDtoStub);
        StringContent data = new(json, Encoding.UTF8, "application/json");
        var request = new HttpRequestMessage(HttpMethod.Put, fixture.Endpoint)
        {
            Content = data
        };

        // Act
        var response = await fixture.DefaultHttpClient.SendAsync(request);

        // Assert
        var errorDetails = await response.DeserializedContentAsync<ErrorDetails>();
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        response.Should().HaveProblemContentType();
        errorDetails!.Errors.Should().ContainMatch($"*{nameof(Client.Email)}*");
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task
        GivenClientDtoWithExistingEmail_WhenRequestedUpdateClient_ThenItShouldReturnConflictWithHeadersAndContent()
    {
        // Arrange
        var json = JsonConvert.SerializeObject(fixture.UpdateClientDtoStub);
        StringContent data = new(json, Encoding.UTF8, "application/json");
        var request = new HttpRequestMessage(HttpMethod.Put, fixture.Endpoint)
        {
            Content = data
        };
        await fixture.DefaultHttpClient.SendAsync(request);

        var requestWithSameEmail = new HttpRequestMessage(HttpMethod.Put, fixture.Endpoint)
        {
            Content = data
        };

        // Act
        var response = await fixture.DefaultHttpClient.SendAsync(requestWithSameEmail);

        // Assert
        var errorDetails = await response.DeserializedContentAsync<ErrorDetails>();
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
        response.Should().HaveProblemContentType();
        errorDetails!.Errors.Should()
            .Contain($"Client with the Email '{fixture.UpdateClientDtoStub.Email}' already exists.");
    }

    //[Fact]
    //[Trait("Category", "Integration")]
    //public async Task
    //    GivenNonExistingClient_WhenRequestedUpdateClient_ThenItShouldReturnNotFoundWithHeadersAndContent()
    //{
    //}
}