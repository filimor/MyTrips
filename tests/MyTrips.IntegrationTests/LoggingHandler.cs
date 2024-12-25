﻿using Serilog;

namespace MyTrips.IntegrationTests;

public class LoggingHandler : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        Log.Information("Sending request: {Method} {Uri}", request.Method, request.RequestUri);

        var response = await base.SendAsync(request, cancellationToken);

        Log.Information("Received response: {StatusCode}", response.StatusCode);
        Log.Information("Response content: {ResponseContent}", response.Content);

        return response;
    }
}