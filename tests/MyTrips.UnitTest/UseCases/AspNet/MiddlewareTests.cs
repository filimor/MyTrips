using Microsoft.Extensions.Hosting;
using MyTrips.Presentation.Middlewares;

namespace MyTrips.UnitTest.UseCases.AspNet;

public class MiddlewareTests
{
    private readonly ExceptionHandlingMiddleware _middleware;

    public MiddlewareTests()
    {
        Mock<IHostEnvironment> environmentMock = new();
        _middleware = new ExceptionHandlingMiddleware(environmentMock.Object);
    }

    //[Fact]
    //[Trait("Category", "Unit")]
    //public async Task GivenExceptionMiddleware_WhenInvoked_ThenItShouldReturnInternalServerErrorAndHeader()
    //{
    //    // Arrange
    //    var context = new DefaultHttpContext();

    //    // Act
    //    await _middleware.InvokeAsync(context, Next);

    //    // Assert
    //    context.Response.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
    //    context.Response.ContentType.Should().Be("application/problem+json; charset=utf-8");
    //}

    //private static Task Next(HttpContext _)
    //{
    //    throw new HttpRequestException();
    //}
}