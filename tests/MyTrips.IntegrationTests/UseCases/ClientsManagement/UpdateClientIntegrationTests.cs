using MyTrips.UnitTest.ClassData;

namespace MyTrips.IntegrationTests.UseCases.ClientsManagement;

[Collection("ClientsManagementIntegration")]
public class UpdateClientIntegrationTests(ClientsManagementFixture fixture)
{
    [Fact]
    [Trait("Category", "Integration")]
    public async Task GivenValidClientDto_WhenRequestedUpdateClient_ThenItShouldReturnSuccessWithHeadersAndContent()
    {
        // Arrange
        var request = fixture.CreateRequest(HttpMethod.Put, fixture.UpdateClientDtoStub);

        // Act
        var response = await fixture.DefaultHttpClient.SendAsync(request);

        // Assert
        var returnedClient = await response.DeserializedContentAsync<ResponseClientDto>();

        response.Should().HaveStatusCode(HttpStatusCode.OK);
        response.Should().HaveJsonContentType();
        returnedClient.Should().BeEquivalentTo(fixture.ResponseClientDtoStub);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task
        GivenClientDtoWithInvalidId_WhenRequestedUpdateClient_ThenItShouldReturnBadRequestWithHeadersAndContent()
    {
        // Arrange
        fixture.UpdateClientDtoStub.Id = -1;
        var request = fixture.CreateRequest(HttpMethod.Put, fixture.UpdateClientDtoStub);

        // Act
        var response = await fixture.DefaultHttpClient.SendAsync(request);

        // Assert
        var errorDetails = await response.DeserializedContentAsync<ErrorDetails>();

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        response.Should().HaveProblemContentType();
        errorDetails!.Errors.Should().ContainMatch($"*{nameof(Client.Id)}*");
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
        var request = fixture.CreateRequest(HttpMethod.Put, fixture.UpdateClientDtoStub);

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
        var request = fixture.CreateRequest(HttpMethod.Put, fixture.UpdateClientDtoStub);

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
        var request = fixture.CreateRequest(HttpMethod.Put, fixture.UpdateClientDtoStub);
        await fixture.DefaultHttpClient.SendAsync(request);
        var requestWithSameEmail = fixture.CreateRequest(HttpMethod.Put, fixture.UpdateClientDtoStub);

        // Act
        var response = await fixture.DefaultHttpClient.SendAsync(requestWithSameEmail);

        // Assert
        var errorDetails = await response.DeserializedContentAsync<ErrorDetails>();

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
        response.Should().HaveProblemContentType();
        errorDetails!.Errors.Should().ContainMatch($"*{nameof(Client.Email)}*{fixture.UpdateClientDtoStub.Email}*");
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task
        GivenNonExistingClient_WhenRequestedUpdateClient_ThenItShouldReturnNotFoundWithHeadersAndContent()
    {
        // Arrange
        fixture.UpdateClientDtoStub.Id = ClientsManagementFixture.NonExistentId;
        var request = fixture.CreateRequest(HttpMethod.Put, fixture.UpdateClientDtoStub);

        // Act
        var response = await fixture.DefaultHttpClient.SendAsync(request);

        // Assert
        var errorDetails = await response.DeserializedContentAsync<ErrorDetails>();

        response.Should().HaveStatusCode(HttpStatusCode.NotFound);
        response.Should().HaveProblemContentType();
        errorDetails!.Errors.Should()
            .ContainMatch($"*{nameof(Client.Id)}*");
    }
}