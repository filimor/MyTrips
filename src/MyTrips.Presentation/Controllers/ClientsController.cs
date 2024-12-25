using Microsoft.AspNetCore.Mvc;
using MyTrips.Application.Interfaces;

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
}