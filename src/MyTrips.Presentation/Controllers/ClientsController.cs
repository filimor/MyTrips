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

    [HttpGet("{id}")]
    public async Task<ActionResult> Get(int id)
    {
        var result = await clientsService.GetClientByIdAsync(id);
        return Ok(result.Value);
    }
}