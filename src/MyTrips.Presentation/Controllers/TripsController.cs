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
public class TripsController(ITripsService tripsService, IValidator<Trip> validator) : ControllerBase
{
    /// <summary>
    /// Get all trips
    /// </summary>
    /// <remarks>
    /// <b>Sample request:</b>
    /// <br />
    /// <pre>
    /// GET /api/trips
    /// </pre>
    /// </remarks>
    /// <param name="getParameters"></param>
    /// <response code="200">Returns all trips</response>
    /// <response code="401">If the user is not authenticated</response>
    /// <response code="429">If the user has sent too many requests in a short period</response>
    /// <response code="500">If an error occurs while processing the request</response>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType<PagedList<ResponseTripDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [Produces("application/json")]
    public async Task<ActionResult> Get([FromQuery] GetParameters getParameters)
    {
        var result = await tripsService.GetTripsAsync(getParameters);

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

    /// <summary>Get a trip by id</summary>
    /// <remarks>
    /// <b>Sample request:</b>
    /// <br />
    /// <pre>
    /// GET /api/trips/1
    /// </pre>
    /// </remarks>
    /// <param name="id"> The id of the trip to get</param>
    /// <returns>A trip with the specified id</returns>
    /// <response code="200">Returns the trip with the specified id</response>
    /// <response code="400">If the id is less than 1</response>
    /// <response code="401">If the user is not authenticated</response>
    /// <response code="404">If the trip with the specified id is not found</response>
    /// <response code="429">If the user has sent too many requests in a short period</response>
    /// <response code="500">If an error occurs while processing the request</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType<ResponseTripDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [Produces("application/json")]
    public async Task<ActionResult> Get([FromRoute] int id)
    {
        var validationResult = ValidateInputId(id);

        if (validationResult.IsFailed)
        {
            var problemDetails = new BadRequestProblemDetails(HttpContext, validationResult);
            return new BadRequestObjectResult(problemDetails);
        }

        var requestResult = await tripsService.GetTripByIdAsync(id);

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

    /// <summary>Book a trip</summary>
    /// <remarks>
    /// <b>Sample request:</b>
    /// <br />
    /// <pre>
    /// POST /api/trips
    /// {
    ///   "clientId": 1,
    ///   "startDate": "2022-12-01",
    ///   "endDate": "2022-12-10",
    ///   "outboundFlightId": 1,
    ///   "inboundFlightId": 2,
    ///   "hotelId": 1
    /// }
    /// </pre>
    /// </remarks>
    /// <param name="createTripDto">The data to book a new trip</param>
    /// <returns>The newly booked trip</returns>
    /// <response code="201">Returns the newly booked trip</response>
    /// <response code="400">If any of the attributes are invalid</response>
    /// <response code="401">If the user is not authenticated</response>
    /// <response code="404">If any of the resources are not found</response>
    /// <response code="429">If the user has sent too many requests in a short period</response>
    /// <response code="500">If an error occurs while processing the request</response>
    [HttpPost]
    [ProducesResponseType<ResponseTripDto>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [Consumes("application/json")]
    [Produces("application/json")]
    public async Task<ActionResult> Post([FromBody] CreateTripDto createTripDto)
    {
        var trip = new Trip(createTripDto.StartDate, createTripDto.EndDate, createTripDto.ClientId,
            createTripDto.InboundFlightId, createTripDto.OutboundFlightId, createTripDto.HotelId);

        var validationResult = await validator.ValidateAsync(trip);

        if (!validationResult.IsValid)
        {
            var problemDetails = new BadRequestProblemDetails(HttpContext, validationResult);
            return new BadRequestObjectResult(problemDetails);
        }

        var requestResult = await tripsService.BookTripAsync(createTripDto);

        if (requestResult.IsFailed)
        {
            if (requestResult.Errors.Any(e => e is NotFoundError))
            {
                var problemDetails = new NotFoundProblemDetails(HttpContext, requestResult.ToResult());

                return new NotFoundObjectResult(problemDetails);
            }

            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        return CreatedAtAction(nameof(Get), new { id = requestResult.Value.Id }, requestResult.Value);
    }

    /// <summary>Cancel a trip</summary>
    /// <remarks>
    /// <b>Sample request:</b>
    /// <br />
    /// <pre>
    /// DELETE /api/trips/1
    /// </pre>
    /// </remarks>
    /// <param name="id">The id of the trip to cancel</param>
    /// <returns>No content</returns>
    /// <response code="204">If the trip was successful cancelled</response>
    /// <response code="400">If the id is less than 1</response>
    /// <response code="401">If the user is not authenticated</response>
    /// <response code="404">If the trip with the specified id is not found</response>
    /// <response code="429">If the user has sent too many requests in a short period</response>
    /// <response code="500">If an error occurs while processing the request</response>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [Produces("application/json")]
    public async Task<ActionResult> Delete([FromRoute] int id)
    {
        var validationResult = ValidateInputId(id);

        if (validationResult.IsFailed)
        {
            var problemDetails = new BadRequestProblemDetails(HttpContext, validationResult);
            return new BadRequestObjectResult(problemDetails);
        }

        var requestResult = await tripsService.CancelTripAsync(id);

        if (requestResult.IsFailed)
        {
            if (requestResult.Errors.Any(e => e is NotFoundError))
            {
                var problemDetails = new NotFoundProblemDetails(HttpContext, requestResult);
                return new NotFoundObjectResult(problemDetails);
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