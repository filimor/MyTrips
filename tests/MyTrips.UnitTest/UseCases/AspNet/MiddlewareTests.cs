﻿using Microsoft.Extensions.Hosting;
using MyTrips.Presentation.Middlewares;

namespace MyTrips.UnitTest.UseCases.AspNet;

public class MiddlewareTests
{
    private readonly Mock<IHostEnvironment> _environmentMock;
    private readonly ExceptionHandlingMiddleware _middleware;

    public MiddlewareTests()
    {
        _environmentMock = new Mock<IHostEnvironment>();
        _middleware = new ExceptionHandlingMiddleware(_environmentMock.Object);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task GivenExceptionMiddleware_WhenInvoked_ThenItShouldReturnInternalServerErrorAndHeader()
    {
        // Arrange
        var context = new DefaultHttpContext();
        _environmentMock.Setup(env => env.EnvironmentName).Returns("Production");

        // Act
        await _middleware.InvokeAsync(context, Next);

        // Assert
        context.Response.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        context.Response.ContentType.Should().Be("application/problem+json; charset=utf-8");
    }

    private Task Next(HttpContext _)
    {
        throw new HttpRequestException();
    }
}