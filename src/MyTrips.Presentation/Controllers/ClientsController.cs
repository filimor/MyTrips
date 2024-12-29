using FluentResults;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using MyTrips.Application.Dtos;
using MyTrips.Application.Errors;
using MyTrips.Application.Interfaces;
using MyTrips.Domain.Entities;
using MyTrips.Domain.ValueObjects;
using MyTrips.Presentation.Errors;
using MyTrips.Presentation.Validators;
using Newtonsoft.Json;

namespace MyTrips.Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
[EnableCors]
[Authorize(AuthenticationSchemes = "JwtScheme")]
public class ClientsController(IClientsService clientsService, IValidator<Client> validator) : ControllerBase
{
    /// <summary>Get all clients</summary>
    /// <remarks>
    /// <b>Sample request:</b>
    /// <br />
    /// <pre>
    /// GET /api/clients/
    /// </pre>
    /// </remarks>
    /// <param name="clientParameters">The parameters for client query.</param>
    /// <returns>A list of all clients</returns>
    /// <response code="200">Returns a list of clients paginated</response>
    /// <response code="500">If an error occurs while processing the request</response>
    [HttpGet]
    [ProducesResponseType<PagedList<ResponseClientDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> Get([FromQuery] ClientParameters clientParameters)
    {
        var result = await clientsService.GetClientsAsync(clientParameters);

        var metadata = new
        {
            result.Value.TotalPages,
            result.Value.PageSize,
            result.Value.CurrentPage,
            result.Value.HasNextPage,
            result.Value.HasPreviousPage
        };

        Response.Headers.Append("X-Pagination", JsonConvert.SerializeObject(metadata));

        return Ok(result.Value);
    }

    /// <summary>Get a client by id</summary>
    /// <remarks>
    /// <b>Sample request:</b>
    /// <br />
    /// <pre>
    /// GET /api/clients/1
    /// </pre>
    /// </remarks>
    /// <param name="id"> The id of the client to get</param>
    /// <returns>A client with the specified id</returns>
    /// <response code="200">Returns the client with the specified id</response>
    /// <response code="400">If the id is less than 1</response>
    /// <response code="401">If the user is not authenticated</response>
    /// <response code="404">If the client with the specified id is not found</response>
    /// <response code="429">If the user has sent too many requests in a short period</response>
    /// <response code="500">If an error occurs while processing the request</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType<ResponseClientDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> Get([FromRoute] int id)
    {
        var validationResult = ValidateInputId(id);

        if (validationResult.IsFailed)
        {
            var problemDetails = new BadRequestProblemDetails(HttpContext, validationResult);
            return new BadRequestObjectResult(problemDetails);
        }

        var requestResult = await clientsService.GetClientByIdAsync(id);

        if (requestResult.IsFailed)
        {
            if (requestResult.Errors.Any(e => e is NotFoundError))
            {
                var problemDetails = new NotFoundProblemDetails(HttpContext, requestResult.ToResult());
                return new NotFoundObjectResult(problemDetails);
            }

            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        return Ok(requestResult.Value);
    }

    /// <summary>Create a new client</summary>
    /// <remarks>
    /// <b>Sample request:</b>
    /// <br />
    /// <pre>
    /// <b>NOTE:</b> The e-mail must be unique.
    /// <br />
    /// POST /api/clients
    /// {
    ///   "name": "John Doe",
    ///   "email": "john.doe@example.com"
    /// }
    /// </pre>
    /// </remarks>
    /// <param name="createClientDto">The data to create a new client</param>
    /// <returns>The newly created client</returns>
    /// <response code="201">Returns the newly created client</response>
    /// <response code="400">If any of the attributes are invalid</response>
    /// <response code="401">If the user is not authenticated</response>
    /// <response code="409">If a client with the same email already exist</response>
    /// <response code="429">If the user has sent too many requests in a short period</response>
    /// <response code="500">If an error occurs while processing the request</response>
    [HttpPost]
    [ProducesResponseType<ResponseClientDto>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [Consumes("application/json")]
    public async Task<ActionResult> Post([FromBody] CreateClientDto createClientDto)
    {
        var client = new Client(createClientDto.Name, createClientDto.Email);

        var validationResult = await validator.ValidateAsync(client);

        if (!validationResult.IsValid)
        {
            var problemDetails = new BadRequestProblemDetails(HttpContext, validationResult);
            return new BadRequestObjectResult(problemDetails);
        }

        var requestResult = await clientsService.AddNewClientAsync(createClientDto);

        if (requestResult.IsFailed)
        {
            if (!requestResult.Errors.Any(e => e is ConflictError))
                return StatusCode(StatusCodes.Status500InternalServerError);

            var problemDetails = new ConflictProblemDetails(HttpContext, requestResult.ToResult());

            return new ConflictObjectResult(problemDetails);
        }

        return CreatedAtAction(nameof(Get), new { id = requestResult.Value.Id }, requestResult.Value);
    }

    /// <summary>Update a client</summary>
    /// <remarks>
    /// <b>Sample request:</b>
    /// <br />
    /// <pre>
    /// PUT /api/clients
    /// {
    ///   "id": 1,
    ///   "name": "Jane Doe",
    ///   "email": "jane.doe@example.com"
    /// }
    /// </pre>
    /// </remarks>
    /// <param name="updateClientDto">The data to update a client</param>
    /// <returns>The updated client</returns>
    /// <response code="200">Returns the updated client</response>
    /// <response code="400">If the id is less than 1 or any of the others attributes are invalid</response>
    /// <response code="401">If the user is not authenticated</response>
    /// <response code="404">If the client with the specified id is not found</response>
    /// <response code="409">If a client with the same id already exist</response>
    /// <response code="429">If the user has sent too many requests in a short period</response>
    /// <response code="500">If an error occurs while processing the request</response>
    [HttpPut]
    [ProducesResponseType<ResponseClientDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [Consumes("application/json")]
    public async Task<ActionResult> Put([FromBody] UpdateClientDto updateClientDto)
    {
        var idValidationResult = ValidateInputId(updateClientDto.Id);

        if (idValidationResult.IsFailed)
        {
            var problemDetails = new BadRequestProblemDetails(HttpContext, idValidationResult);
            return new BadRequestObjectResult(problemDetails);
        }

        var client = new Client(updateClientDto.Id, updateClientDto.Name, updateClientDto.Email);

        var validationResult = await validator.ValidateAsync(client);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => new Error(e.ErrorMessage));
            var resultObject = Result.Fail(errors);
            var problemDetails = new BadRequestProblemDetails(HttpContext, resultObject);
            return new BadRequestObjectResult(problemDetails);
        }

        var requestResult = await clientsService.UpdateClientAsync(updateClientDto);

        if (requestResult.IsFailed)
        {
            if (requestResult.Errors.Any(e => e is NotFoundError))
            {
                var problemDetails = new NotFoundProblemDetails(HttpContext, requestResult.ToResult());
                return new NotFoundObjectResult(problemDetails);
            }

            if (requestResult.Errors.Any(e => e is ConflictError))
            {
                var problemDetails = new ConflictProblemDetails(HttpContext, requestResult.ToResult());
                return new ConflictObjectResult(problemDetails);
            }

            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        return Ok(requestResult.Value);
    }

    /// <summary>Delete a client</summary>
    /// <remarks>
    /// <b>Sample request:</b>
    /// <br />
    /// <pre>
    /// DELETE /api/clients/1
    /// </pre>
    /// </remarks>
    /// <param name="id">The id of the client to delete</param>
    /// <returns>No content</returns>
    /// <response code="204">If the client was successful deleted</response>
    /// <response code="400">If the id is less than 1</response>
    /// <response code="401">If the user is not authenticated</response>
    /// <response code="404">If the client with the specified id is not found</response>
    /// <response code="409">If the client is referenced by another entity</response>
    /// <response code="429">If the user has sent too many requests in a short period</response>
    /// <response code="500">If an error occurs while processing the request</response>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> Delete([FromRoute] int id)
    {
        var validationResult = ValidateInputId(id);
        if (validationResult.IsFailed)
        {
            var problemDetails = new BadRequestProblemDetails(HttpContext, validationResult);
            return new BadRequestObjectResult(problemDetails);
        }

        var requestResult = await clientsService.RemoveClientAsync(id);

        if (requestResult.IsFailed)
        {
            if (requestResult.Errors.Any(e => e is NotFoundError))
            {
                var problemDetails = new NotFoundProblemDetails(HttpContext, requestResult);
                return new NotFoundObjectResult(problemDetails);
            }

            if (requestResult.Errors.Any(e => e is ConflictError))
            {
                var problemDetails = new ConflictProblemDetails(HttpContext, requestResult);
                return new ConflictObjectResult(problemDetails);
            }

            return StatusCode(StatusCodes.Status500InternalServerError);
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