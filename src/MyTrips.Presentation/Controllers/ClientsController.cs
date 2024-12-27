
using FluentResults;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using MyTrips.Application.Dtos;
using MyTrips.Application.Interfaces;
using MyTrips.Domain.Entities;
using MyTrips.Presentation.Errors;
using MyTrips.Presentation.Validators;

namespace MyTrips.Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ClientsController(IClientsService clientsService, IValidator<Client> validator) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<IEnumerable<ResponseClientDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorDetails>(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> Get()
    {
        var result = await clientsService.GetClientsAsync();

        return Ok(result.Value);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType<ResponseClientDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ErrorDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ErrorDetails>(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> Get([FromRoute] int id)
    {
        var validationResult = ValidateInputId(id);

        if (validationResult.IsFailed)
        {
            var errorDetails = new BadRequestErrorDetails(HttpContext, validationResult);
            return new BadRequestObjectResult(errorDetails);
        }

        var requestResult = await clientsService.GetClientByIdAsync(id);

        if (requestResult.IsFailed)
        {
            var errorDetails = new NotFoundErrorDetails(HttpContext, requestResult.ToResult());
            return new NotFoundObjectResult(errorDetails);
        }

        return Ok(requestResult.Value);
    }

    [HttpPost]
    [ProducesResponseType<ResponseClientDto>(StatusCodes.Status201Created)]
    [ProducesResponseType<ErrorDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ErrorDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ErrorDetails>(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> Post([FromBody] CreateClientDto createClientDto)
    {
        var client = new Client(createClientDto.Name, createClientDto.Email);

        var validationResult = await validator.ValidateAsync(client);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => new Error(e.ErrorMessage));
            var resultObject = Result.Fail(errors);
            var errorDetails = new BadRequestErrorDetails(HttpContext, resultObject);
            return new BadRequestObjectResult(errorDetails);
        }

        var requestResult = await clientsService.AddNewClientAsync(createClientDto);

        if (requestResult.IsFailed)
        {
            var errorDetails = new ConflictErrorDetails(HttpContext, requestResult.ToResult());
            return new ConflictObjectResult(errorDetails);
        }

        return CreatedAtAction(nameof(Get), new { id = requestResult.Value.Id }, requestResult.Value);
    }

    [HttpPut]
    [ProducesResponseType<ResponseClientDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ErrorDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ErrorDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ErrorDetails>(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> Put([FromBody] UpdateClientDto updateClientDto)
    {
        var client = new Client(updateClientDto.Id, updateClientDto.Name, updateClientDto.Email);

        var validationResult = await validator.ValidateAsync(client);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => new Error(e.ErrorMessage));
            var resultObject = Result.Fail(errors);
            var errorDetails = new BadRequestErrorDetails(HttpContext, resultObject);
            return new BadRequestObjectResult(errorDetails);
        }

        return NoContent();
    }

    private static Result ValidateInputId(int id)
    {
        var validationResult = InputValidator.ValidateId(id);

        if (id >= 1) return Result.Ok();

        var errors = validationResult.Errors.Select(e => new Error(e.ErrorMessage));
        var resultObject = Result.Fail(errors);

        return resultObject;
    }
}