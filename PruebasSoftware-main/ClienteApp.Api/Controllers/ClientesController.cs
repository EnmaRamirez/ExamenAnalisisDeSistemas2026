using ClienteApp.Core.Models;
using ClienteApp.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace ClienteApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClientesController : ControllerBase
{
    private readonly ClienteService _service;

    public ClientesController(ClienteService service)
    {
        _service = service;
    }

    [HttpPost]
    public ActionResult<Cliente> Registrar(
        [FromBody] RegistrarClienteRequest request)
    {
        try
        {
            var cliente = _service.Registrar(
                request.Nombre,
                request.Correo);

            return CreatedAtAction(
                nameof(ObtenerPorId),
                new { id = cliente.Id },
                cliente);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { mensaje = ex.Message });
        }
    }

    [HttpGet("{id:int}")]
    public ActionResult<Cliente> ObtenerPorId(int id)
    {
        var cliente = _service.ObtenerPorId(id);

        if (cliente is null)
        {
            return NotFound(new { mensaje = "Cliente no encontrado." });
        }

        return Ok(cliente);
    }
}

public class RegistrarClienteRequest
{
    public string Nombre { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;
}