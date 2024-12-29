using FluentResults;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;

namespace MyTrips.Presentation.Errors;

public class BadRequestProblemDetails : ProblemDetails
{
    private BadRequestProblemDetails(HttpContext context, string? detail)
    {
        Status = StatusCodes.Status400BadRequest;
        Title = "Bad Request";
        Detail = detail ??
                 "The server cannot process the request due to a client error. This could be due to a malformed request syntax, invalid request message framing, or deceptive request routing. Please check the request data and ensure it follows the correct format and try again.";
        Instance = context.Request.Path;
        Type = $"https://httpstatuses.com/{Status}";
    }

    public BadRequestProblemDetails(HttpContext context, ValidationResult validationResult,
        string? detail = null) : this(context, detail)
    {
        Extensions.Add("Errors", validationResult.Errors.ToDictionary(e => e.PropertyName, e => e.ErrorMessage));
    }

    public BadRequestProblemDetails(HttpContext context, Result fluentResult,
        string? detail = null) : this(context, detail)
    {
        Extensions.Add("Errors", fluentResult.Errors.ToDictionary(e => e.Message, e => e.Reasons));
    }
}