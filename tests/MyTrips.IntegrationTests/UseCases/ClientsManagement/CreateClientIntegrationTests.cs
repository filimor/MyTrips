using Microsoft.Data.SqlClient;
using MyTrips.UnitTest.ClassData;
using RepoDb;

namespace MyTrips.IntegrationTests.UseCases.ClientsManagement;

[Collection("ClientsManagementIntegration")]
public class CreateClientIntegrationTests(ClientsManagementFixture fixture)
{
    [Fact]
    [Trait("Category", "Integration")]
    public async Task GivenValidClientDto_WhenRequestedPostClient_ThenItShouldReturnCreatedWithHeadersAndContent()
    {
        // Arrange
        var request = fixture.CreateRequest(HttpMethod.Post, fixture.CreateClientDtoStub);

        // Act
        var response = await fixture.DefaultHttpClient.SendAsync(request);

        // Assert
        var returnedClient = await response.DeserializedContentAsync<ResponseClientDto>();

        response.Should().HaveStatusCode(HttpStatusCode.Created);
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
        fixture.CreateClientDtoStub.Name = name;
        var request = fixture.CreateRequest(HttpMethod.Post, fixture.CreateClientDtoStub);

        // Act
        var response = await fixture.DefaultHttpClient.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        response.Should().HaveProblemContentType();
    }

    [Theory]
    [Trait("Category", "Integration")]
    [ClassData(typeof(InvalidEmailClassData))]
    public async Task
        GivenClientDtoWithInvalidEmail_WhenRequestedPostClient_ThenItShouldReturnBadRequestWithHeadersAndContent(
            string email)
    {
        // Arrange
        fixture.CreateClientDtoStub.Email = email;
        var request = fixture.CreateRequest(HttpMethod.Post, fixture.CreateClientDtoStub);

        // Act
        var response = await fixture.DefaultHttpClient.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        response.Should().HaveProblemContentType();
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task
        GivenClientDtoWithExistingEmail_WhenRequestedPostClient_ThenItShouldReturnConflictWithHeadersAndContent()
    {
        // Arrange
        var request = fixture.CreateRequest(HttpMethod.Post, fixture.CreateClientDtoStub);
        await fixture.DefaultHttpClient.SendAsync(request);
        var requestWithSameEmail = fixture.CreateRequest(HttpMethod.Post, fixture.CreateClientDtoStub);

        // Act
        var response = await fixture.DefaultHttpClient.SendAsync(requestWithSameEmail);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
        response.Should().HaveProblemContentType();
    }

    [Theory]
    [Trait("Category", "Integration")]
    [ClassData(typeof(InvalidStringClassData))]
    public async Task
        GivenClientDtoWithInvalidName_WhenRequestedPostClient_ThenItShouldNotPersistIt(string name)
    {
        // Arrange
        fixture.CreateClientDtoStub.Name = name;
        var request = fixture.CreateRequest(HttpMethod.Post, fixture.CreateClientDtoStub);

        // Act
        await fixture.DefaultHttpClient.SendAsync(request);

        // Assert
        await using var connection = new SqlConnection(fixture.ConnectionString);
        var clients = await connection.QueryAllAsync<Client>();

        clients.Should().NotContain(c => c.Name == fixture.CreateClientDtoStub.Name);
    }

    [Theory]
    [Trait("Category", "Integration")]
    [ClassData(typeof(InvalidEmailClassData))]
    public async Task
        GivenClientDtoWithInvalidEmail_WhenRequestedPostClient_ThenItShouldNotPersistIt(string email)
    {
        // Arrange
        fixture.CreateClientDtoStub.Email = email;
        var request = fixture.CreateRequest(HttpMethod.Post, fixture.CreateClientDtoStub);

        // Act
        await fixture.DefaultHttpClient.SendAsync(request);

        // Assert
        await using var connection = new SqlConnection(fixture.ConnectionString);
        var clients = await connection.QueryAllAsync<Client>();

        clients.Should().NotContain(c => c.Email == fixture.CreateClientDtoStub.Email);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task
        GivenClientDtoWithExistingEmail_WhenRequestedPostClient_ThenItShouldNotPersistItAgain()
    {
        // Arrange
        var request = fixture.CreateRequest(HttpMethod.Post, fixture.CreateClientDtoStub);
        await fixture.DefaultHttpClient.SendAsync(request);
        var requestWithSameEmail = fixture.CreateRequest(HttpMethod.Post, fixture.CreateClientDtoStub);

        // Act
        await fixture.DefaultHttpClient.SendAsync(requestWithSameEmail);

        // Assert
        await using var connection = new SqlConnection(fixture.ConnectionString);
        var clients = await connection.QueryAllAsync<Client>();

        clients.Should().ContainSingle(c => c.Email == fixture.CreateClientDtoStub.Email);
    }
}