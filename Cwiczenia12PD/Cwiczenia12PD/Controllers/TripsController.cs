using Cwiczenia12PD.Models.DTOs;
using Cwiczenia12PD.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cwiczenia12PD.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TripsController : ControllerBase
{
    private readonly IDbService _service;

    public TripsController(IDbService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetTrips([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var result = await _service.GetTripsAsync(page, pageSize);
        return Ok(result);
    }
    
    [HttpPost("{idTrip}/clients")]
    public async Task<IActionResult> PostClient(int idTrip, [FromBody] AssignClientToTripDto dto)
    {
        try
        {
            var response = await _service.AssignClientToTripAsync(idTrip, dto);
            return CreatedAtAction(nameof(GetTrips), new { idTrip }, response);
        }
        catch (KeyNotFoundException knf)
        {
            return NotFound(new { message = knf.Message });
        }
        catch (InvalidOperationException ioe)
        {
            return BadRequest(new { message = ioe.Message });
        }
    }
}