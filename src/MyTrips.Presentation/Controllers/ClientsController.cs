using FluentResults;
using Microsoft.AspNetCore.Mvc;
using MyTrips.Application.Interfaces;
using MyTrips.Presentation.Errors;
using MyTrips.Presentation.Validators;

namespace MyTrips.Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ClientsController(IClientsService clientsService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult> Get()
    {
        var result = await clientsService.GetClientsAsync();
        return Ok(result.Value);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> Get(int id)
    {
        var validationResult = InputValidator.ValidateId(id);

        if (id < 1)
        {
            var errors = validationResult.Errors.Select(e => new Error(e.ErrorMessage));
            var resultObject = Result.Fail(errors);
            var errorDetails = new ErrorDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Bad Request",
                Instance = HttpContext.Request.Path,
                Detail = "The input data is incorrect.",
                Errors = resultObject.Errors.Select(e => e.Message).ToArray()
            };
            var badRequestResult = new BadRequestObjectResult(errorDetails)
            {
                ContentTypes = { "application/problem+json; charset=utf-8" }
            };
            return badRequestResult;
        }

        var requestResult = await clientsService.GetClientByIdAsync(id);

        if (requestResult.IsFailed)
        {
            var errorDetails = new ErrorDetails
            {
                Status = StatusCodes.Status404NotFound,
                Title = "Not Found",
                Instance = HttpContext.Request.Path,
                Detail = "",
                Errors = requestResult.Errors.Select(e => e.Message).ToArray()
            };

            var notFoundResult = new NotFoundObjectResult(errorDetails)
            {
                ContentTypes = { "application/problem+json; charset=utf-8" }
            };

            return notFoundResult;
        }

        return Ok(requestResult.Value);
    }
}