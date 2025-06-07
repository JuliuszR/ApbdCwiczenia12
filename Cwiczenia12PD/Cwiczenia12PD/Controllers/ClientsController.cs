using Cwiczenia12PD.Data;
using Cwiczenia12PD.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cwiczenia12PD.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClientsController : ControllerBase
{
    private readonly IDbService _service;

    public ClientsController(IDbService service)
    {
        _service = service;
    }

    [HttpDelete("{idClient}")]
    public async Task<IActionResult> Delete(int idClient)
    {
        try
        {
            await _service.DeleteClientAsync(idClient);
            return NoContent();
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