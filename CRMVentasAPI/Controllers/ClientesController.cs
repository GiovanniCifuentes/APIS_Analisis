// Controllers/ClientesController.cs
using Microsoft.AspNetCore.Mvc;
using CRMVentasAPI.Services;
using CRMVentasAPI.Models;

namespace CRMVentasAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientesController : ControllerBase
    {
        private readonly IExternalApiService _externalApiService;

        public ClientesController(IExternalApiService externalApiService)
        {
            _externalApiService = externalApiService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ExternalCliente>>> GetClientes()
        {
            try
            {
                var clientes = await _externalApiService.GetClientesAsync();
                return Ok(clientes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = $"Error al obtener clientes: {ex.Message}" });
            }
        }

        [HttpGet("activos")]
        public async Task<ActionResult<IEnumerable<ExternalCliente>>> GetClientesActivos()
        {
            try
            {
                var clientes = await _externalApiService.GetClientesAsync();
                var activos = clientes.Where(c => c.Estado).ToList();
                return Ok(activos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = $"Error al obtener clientes activos: {ex.Message}" });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ExternalCliente>> GetClienteById(string id)
        {
            try
            {
                var clientes = await _externalApiService.GetClientesAsync();
                var cliente = clientes.FirstOrDefault(c => c.Id == id);

                if (cliente == null)
                    return NotFound(new { mensaje = $"Cliente con ID {id} no encontrado" });

                return Ok(cliente);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = $"Error al obtener cliente: {ex.Message}" });
            }
        }
    }
}