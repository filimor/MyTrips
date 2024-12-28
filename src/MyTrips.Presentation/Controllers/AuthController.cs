using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyTrips.Application.Dtos;
using MyTrips.Application.Interfaces;
using MyTrips.Domain.Entities;
using MyTrips.Presentation.Errors;

namespace MyTrips.Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController(IAuthService authService) : ControllerBase
{
    /// <summary>
    /// Login
    /// </summary>
    /// <remarks>
    /// <b>Sample request:</b>
    /// <br />
    /// <pre>
    /// POST /Login
    /// {
    ///   "username": "admin",
    ///   "password": "admin"
    /// }
    /// </pre>
    /// </remarks>
    /// <param name="loginInfo">
    /// The login information
    /// </param>
    /// <returns>
    /// The token
    /// </returns>
    /// <response code="200">Returns the token</response>
    /// <response code="401">If the credentials are invalid</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpPost("Login")]
    [AllowAnonymous]
    [ProducesResponseType<IEnumerable<ResponseClientDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ErrorDetails>(StatusCodes.Status500InternalServerError)]
    public IActionResult Login(LoginInfo loginInfo)
    {
        var token = authService.GetToken(loginInfo);

        if (string.IsNullOrEmpty(token)) return new UnauthorizedObjectResult(new UnauthorizedErrorDetails(HttpContext));

        return Ok(new
        {
            Token = token
        });
    }
}