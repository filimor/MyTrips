using Serilog;

namespace MyTrips.IntegrationTests.Handlers;

public class LoggingHandler : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        Log.Information("##### SENDING REQUEST #####: {Method} {Uri}", request.Method, request.RequestUri);

        var response = await base.SendAsync(request, cancellationToken);

        Log.Information("##### RECEIVED RESPONSE #####: {StatusCode}", response.StatusCode);

        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

        Log.Information("##### RESPONSE CONTENT #####: {ResponseContent}", responseContent
        );

        return response;
    }
}