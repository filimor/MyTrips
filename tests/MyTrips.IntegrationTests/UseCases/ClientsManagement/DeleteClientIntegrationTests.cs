using Microsoft.Data.SqlClient;
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
        var returnedClient = await CreateClient();
        var request = fixture.CreateRequest(HttpMethod.Delete, endpoint: $"{fixture.Endpoint}/{returnedClient!.Id}");

        // Act
        var response = await fixture.DefaultHttpClient.SendAsync(request);

        // Assert
        var responseContent = await response.DeserializedContentAsync<object>();

        response.Should().HaveStatusCode(HttpStatusCode.NoContent);
        responseContent.Should().BeNull();
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task GivenInvalidId_WhenRequestDeleteClient_ThenItShouldReturnBadRequestWithErrorsAndProblemHeader()
    {
        // Arrange
        var request = fixture.CreateRequest(HttpMethod.Delete,
            endpoint: $"{fixture.Endpoint}/{ClientsManagementFixture.InvalidId}");

        // Act
        var response = await fixture.DefaultHttpClient.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        response.Should().HaveProblemContentType();
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task
        GivenNonExistentClient_WhenRequestDeleteClient_ThenItShouldReturnNotFoundWithErrorsAndProblemHeader()
    {
        // Arrange
        var request = fixture.CreateRequest(HttpMethod.Delete, endpoint:
            $"{fixture.Endpoint}/{ClientsManagementFixture.NonExistentId}");

        // Act
        var response = await fixture.DefaultHttpClient.SendAsync(request);

        // Assert
        response.Should().HaveStatusCode(HttpStatusCode.NotFound);
        response.Should().HaveProblemContentType();
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task GivenAnExistingClient_WhenRequestDeleteClient_ThenItShouldDeleteClientFromDatabase()
    {
        // Arrange
        var returnedClient = await CreateClient();
        var request = fixture.CreateRequest(HttpMethod.Delete, $"{fixture.Endpoint}/{returnedClient!.Id}");

        // Act
        await fixture.DefaultHttpClient.SendAsync(request);

        // Assert
        await using var connection = new SqlConnection(fixture.ConnectionString);
        var clients = await connection.QueryAsync<Client>(c => c.Id == returnedClient.Id);

        clients.Should().BeEmpty();
    }

    private async Task<ResponseClientDto?> CreateClient()
    {
        var createRequest = fixture.CreateRequest(HttpMethod.Post,
            fixture.CreateClientDtoStub);
        var createResponse = await fixture.DefaultHttpClient.SendAsync(createRequest);
        var returnedClient = await createResponse.DeserializedContentAsync<ResponseClientDto>();

        return returnedClient;
    }
}