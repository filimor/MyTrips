using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Moq;
using MyTrips.Presentation.Filters;

namespace MyTrips.UnitTest.UseCases.AspNet;

public class FilterTests
{
    [Theory]
    [InlineData(StatusCodes.Status400BadRequest)]
    [InlineData(StatusCodes.Status500InternalServerError)]
    [Trait("Category", "Unit")]
    public void GivenErrorStatusCode_WhenExecuteProblemHeaderFilter_ThenItShouldAddProblemContentTypeHeader(
        int statusCode)
    {
        // Arrange
        var filter = new ProblemHeaderFilter();

        var context = new ActionExecutedContext(
            new ActionContext
            {
                HttpContext = new DefaultHttpContext(),
                RouteData = new RouteData(),
                ActionDescriptor = new ControllerActionDescriptor()
            },
            [],
            new Mock<Controller>().Object
        )
        {
            Result = new ObjectResult(null) { StatusCode = statusCode }
        };

        // Act
        filter.OnActionExecuted(context);

        // Assert
        context.HttpContext.Response.Headers.ContentType.Should()
            .BeEquivalentTo("application/problem+json; charset=utf-8");
    }

    [Theory]
    [InlineData(StatusCodes.Status100Continue)]
    [InlineData(StatusCodes.Status200OK)]
    [InlineData(StatusCodes.Status300MultipleChoices)]
    [Trait("Category", "Unit")]
    public void GivenNonErrorStatusCode_WhenExecuteProblemHeaderFilter_ThenItShouldNotAddErrorContentTypeHeader(
        int statusCode)
    {
        // Arrange
        var filter = new ProblemHeaderFilter();

        var context = new ActionExecutedContext(
            new ActionContext
            {
                HttpContext = new DefaultHttpContext(),
                RouteData = new RouteData(),
                ActionDescriptor = new ControllerActionDescriptor()
            },
            [],
            new Mock<Controller>().Object
        )
        {
            Result = new ObjectResult(null) { StatusCode = statusCode }
        };

        // Act
        filter.OnActionExecuted(context);

        // Assert
        context.HttpContext.Response.Headers.ContentType.Should()
            .NotBeEquivalentTo("application/problem+json; charset=utf-8");
    }
}