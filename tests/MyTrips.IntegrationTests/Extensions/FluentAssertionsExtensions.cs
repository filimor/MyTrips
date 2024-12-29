using FluentAssertions.Primitives;

namespace MyTrips.IntegrationTests.Extensions;

public static class FluentAssertionsExtensions
{
    public static AndConstraint<HttpResponseMessageAssertions> HaveJsonContentType(
        this HttpResponseMessageAssertions assertions)
    {
        var contentType = assertions.Subject.Content.Headers.ContentType?.ToString();
        contentType.Should().Be("application/json; charset=utf-8", "the Content-Type should be JSON");

        return new AndConstraint<HttpResponseMessageAssertions>(assertions);
    }

    public static AndConstraint<HttpResponseMessageAssertions> HaveProblemContentType(
        this HttpResponseMessageAssertions assertions)
    {
        var contentType = assertions.Subject.Content.Headers.ContentType?.ToString();
        contentType.Should().Be("application/problem+json; charset=utf-8", "RFC 7807");

        return new AndConstraint<HttpResponseMessageAssertions>(assertions);
    }
}