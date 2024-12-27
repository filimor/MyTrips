using System.Net;
using System.Text;
using FluentAssertions;
using MyTrips.Application.Dtos;
using MyTrips.IntegrationTests.Extensions;
using MyTrips.IntegrationTests.Fixtures;
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

    //[Theory]
    //[Trait("Category", "Integration")]
    //[ClassData(typeof(InvalidStringClassData))]
    //public async Task
    //    GivenClientDtoWithInvalidName_WhenRequestedUpdateClient_ThenItShouldReturnBadRequestWithHeadersAndContent(
    //        string name)
    //{
    //}

    //[Theory]
    //[Trait("Category", "Integration")]
    //[ClassData(typeof(InvalidEmailClassData))]
    //public async Task
    //    GivenClientDtoWithInvalidEmail_WhenRequestedUpdateClient_ThenItShouldReturnBadRequestWithHeadersAndContent(
    //        string email)
    //{
    //}

    //[Fact]
    //[Trait("Category", "Integration")]
    //public async Task
    //    GivenClientDtoWithExistingEmail_WhenRequestedUpdateClient_ThenItShouldReturnConflictWithHeadersAndContent()
    //{
    //}

    //[Fact]
    //[Trait("Category", "Integration")]
    //public async Task
    //    GivenNonExistingClient_WhenRequestedUpdateClient_ThenItShouldReturnNotFoundWithHeadersAndContent()
    //{
    //}
}