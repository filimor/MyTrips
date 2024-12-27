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

[Collection("ClientsManagement")]
public class CreateClientIntegrationTests(ClientsManagementFixture fixture)
{
    [Fact]
    [Trait("Category", "Integration")]
    public async Task GivenValidClientDto_WhenRequestedPostClient_ThenItShouldReturnCreatedWithHeadersAndContent()
    {
        // Arrange
        var json = JsonConvert.SerializeObject(fixture.RequestClientDtoStub);
        StringContent data = new(json, Encoding.UTF8, "application/json");
        var request = new HttpRequestMessage(HttpMethod.Post, fixture.Endpoint)
        {
            Content = data
        };

        // Act
        var response = await fixture.DefaultHttpClient.SendAsync(request);

        // Assert
        var returnedClient = await response.DeserializedContentAsync<ResponseClientDto>();
        response.EnsureSuccessStatusCode();
        response.Should().HaveJsonContentType();
        returnedClient.Should().BeEquivalentTo(fixture.ResponseClientDtoStub, options => options.Excluding(c => c.Id));
        returnedClient!.Id.Should().BeGreaterThan(0);
    }

    [Theory]
    [Trait("Category", "Integration")]
    [ClassData(typeof(InvalidStringClassData))]
    public async Task
        GivenClientDtoWithInvalidName_WhenRequestedPostClient_ThenItShouldReturnBadRequestWithHeadersAndContent(
            string name)
    {
        // Arrange
        fixture.RequestClientDtoStub.Name = name;
        var json = JsonConvert.SerializeObject(fixture.RequestClientDtoStub);
        StringContent data = new(json, Encoding.UTF8, "application/json");
        var request = new HttpRequestMessage(HttpMethod.Post, fixture.Endpoint)
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
        GivenClientDtoWithInvalidEmail_WhenRequestedPostClient_ThenItShouldReturnBadRequestWithHeadersAndContent(
            string email)
    {
        // Arrange
        fixture.RequestClientDtoStub.Email = email;
        var json = JsonConvert.SerializeObject(fixture.RequestClientDtoStub);
        StringContent data = new(json, Encoding.UTF8, "application/json");
        var request = new HttpRequestMessage(HttpMethod.Post, fixture.Endpoint)
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

    //[Fact]
    //[Trait("Category", "Integration")]
    //public async Task
    //    GivenClientDtoWithExistingEmail_WhenRequestedPostClient_ThenItShouldReturnConflictWithHeadersAndContent()
    //{
    //}
}