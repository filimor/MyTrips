using System.Text;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using RepoDb;

namespace MyTrips.IntegrationTests.UseCases.ClientsManagement;

[Collection("ClientsManagementIntegration")]
public class DeleteClientIntegrationTests(ClientsManagementFixture fixture)
{
    [Fact]
    [Trait("Category", "Integration")]
    public async Task GivenAnExistingClient_WhenRequestDeleteClient_ThenItShouldReturnNoContent()
    {
        // Arrange

        var json = JsonConvert.SerializeObject(fixture.CreateClientDtoStub);
        StringContent data = new(json, Encoding.UTF8, "application/json");
        var createRequest = new HttpRequestMessage(HttpMethod.Post, fixture.Endpoint)
        {
            Content = data
        };
        createRequest.Headers.Authorization = fixture.GetAuthorizationHeader();
        var createResponse = await fixture.DefaultHttpClient.SendAsync(createRequest);
        var returnedClient = await createResponse.DeserializedContentAsync<ResponseClientDto>();

        var request = new HttpRequestMessage(HttpMethod.Delete, $"{fixture.Endpoint}/{returnedClient!.Id}");
        request.Headers.Authorization = fixture.GetAuthorizationHeader();

        // Act
        var response = await fixture.DefaultHttpClient.SendAsync(request);

        // Assert
        var deleteResponse = await response.DeserializedContentAsync<object>();

        response.Should().HaveStatusCode(HttpStatusCode.NoContent);
        deleteResponse.Should().BeNull();
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task GivenInvalidId_WhenRequestDeleteClient_ThenItShouldReturnBadRequestWithErrorsAndProblemHeader()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Delete, $"{fixture.Endpoint}/0");
        request.Headers.Authorization = fixture.GetAuthorizationHeader();

        // Act
        var response = await fixture.DefaultHttpClient.SendAsync(request);

        // Assert
        var errorDetails = await response.DeserializedContentAsync<ErrorDetails>();
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        response.Should().HaveProblemContentType();
        errorDetails!.Errors.Should().ContainMatch($"*{nameof(Client.Id)}*");
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task
        GivenNonExistentClient_WhenRequestDeleteClient_ThenItShouldReturnNotFoundWithErrorsAndProblemHeader()
    {
        // Arrange

        var nonExistentId = int.MaxValue;
        var request = new HttpRequestMessage(HttpMethod.Delete, $"{fixture.Endpoint}/{nonExistentId}");
        request.Headers.Authorization = fixture.GetAuthorizationHeader();

        // Act
        var response = await fixture.DefaultHttpClient.SendAsync(request);

        // Assert
        var errorDetails = await response.DeserializedContentAsync<ErrorDetails>();
        response.Should().HaveStatusCode(HttpStatusCode.NotFound);
        response.Should().HaveProblemContentType();
        errorDetails!.Errors.Should()
            .Contain($"{nameof(Client)} with {nameof(Client.Id)} '{nonExistentId}' not found.");
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task GivenAnExistingClient_WhenRequestDeleteClient_ThenItShouldDeleteClientFromDatabase()
    {
        // Arrange

        var json = JsonConvert.SerializeObject(fixture.CreateClientDtoStub);
        StringContent data = new(json, Encoding.UTF8, "application/json");
        var createRequest = new HttpRequestMessage(HttpMethod.Post, fixture.Endpoint)
        {
            Content = data
        };
        createRequest.Headers.Authorization = fixture.GetAuthorizationHeader();
        var createResponse = await fixture.DefaultHttpClient.SendAsync(createRequest);
        var returnedClient = await createResponse.DeserializedContentAsync<ResponseClientDto>();

        var request = new HttpRequestMessage(HttpMethod.Delete, $"{fixture.Endpoint}/{returnedClient!.Id}");
        request.Headers.Authorization = fixture.GetAuthorizationHeader();

        // Act
        await fixture.DefaultHttpClient.SendAsync(request);

        // Assert
        await using var connection = new SqlConnection(fixture.ConnectionString);
        var clients = await connection.QueryAsync<Client>(c => c.Id == returnedClient.Id);
        clients.Should().BeEmpty();
    }
}