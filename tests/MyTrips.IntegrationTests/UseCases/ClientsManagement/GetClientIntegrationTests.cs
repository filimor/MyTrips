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

        const int existingId = 1;
        var request = new HttpRequestMessage(HttpMethod.Get, $"{fixture.Endpoint}/{existingId}");
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

        const int invalidId = -1;
        const int minId = 1;
        var request = new HttpRequestMessage(HttpMethod.Get, $"{fixture.Endpoint}/{invalidId}");
        request.Headers.Authorization = fixture.GetAuthorizationHeader();

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

        const int nonExistentId = int.MaxValue;
        var request = new HttpRequestMessage(HttpMethod.Get, $"{fixture.Endpoint}/{nonExistentId}");
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
}