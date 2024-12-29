namespace MyTrips.IntegrationTests.UseCases.ClientsManagement;

[Collection("ClientsManagementIntegration")]
public class GetClientIntegrationTests(ClientsManagementFixture fixture)
{
    [Fact]
    [Trait("Category", "Integration")]
    public async Task GivenClientsEndpoint_WhenRequestedGetClientWithoutId_ThenItShouldReturnOkWithHeadersAndContent()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Get, fixture.Endpoint);
        request.Headers.Authorization = fixture.GetAuthorizationHeader();

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
        var request = new HttpRequestMessage(HttpMethod.Get, $"{fixture.Endpoint}/{ClientsManagementFixture.MinId}");
        request.Headers.Authorization = fixture.GetAuthorizationHeader();

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
        var request =
            new HttpRequestMessage(HttpMethod.Get, $"{fixture.Endpoint}/{ClientsManagementFixture.InvalidId}");
        request.Headers.Authorization = fixture.GetAuthorizationHeader();

        // Act
        var response = await fixture.DefaultHttpClient.SendAsync(request);

        // Assert
        var errorDetails = await response.DeserializedContentAsync<ErrorDetails>();

        response.Should().HaveStatusCode(HttpStatusCode.BadRequest);
        response.Should().HaveProblemContentType();
        errorDetails!.Errors.Should().ContainMatch($"*{nameof(Client.Id)}*");
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task
        GivenNonExistentClient_WhenRequestGetClientWithId_ThenItShouldReturnNotFoundWithHeadersAndContent()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Get,
            $"{fixture.Endpoint}/{ClientsManagementFixture.NonExistentId}");
        request.Headers.Authorization = fixture.GetAuthorizationHeader();

        // Act
        var response = await fixture.DefaultHttpClient.SendAsync(request);

        // Assert
        var errorDetails = await response.DeserializedContentAsync<ErrorDetails>();

        response.Should().HaveStatusCode(HttpStatusCode.NotFound);
        response.Should().HaveProblemContentType();
        errorDetails!.Errors.Should().ContainMatch($"*{nameof(Client.Id)}*");
    }
}