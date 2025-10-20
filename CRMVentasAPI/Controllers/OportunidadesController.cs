using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CRMVentasAPI;
using CRMVentasAPI.Models;
using CRMVentasAPI.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CRMVentasAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OportunidadesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IExternalApiService _externalApiService;
        private readonly ILogger<OportunidadesController> _logger;

        public OportunidadesController(AppDbContext context, IExternalApiService externalApiService, ILogger<OportunidadesController> logger)
        {
            _context = context;
            _externalApiService = externalApiService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<Oportunidad>> Crear([FromBody] Oportunidad oportunidad)
        {
            // Validación mínima del payload
            if (oportunidad == null)
            {
                return BadRequest(new { mensaje = "Payload vacío" });
            }
            if (string.IsNullOrWhiteSpace(oportunidad.Titulo))
            {
                return BadRequest(new { mensaje = "El título es requerido" });
            }
            if (string.IsNullOrWhiteSpace(oportunidad.ClienteId))
            {
                return BadRequest(new { mensaje = "El clienteId es requerido y debe ser el id del cliente externo" });
            }

            // Validar que el cliente existe en la API externa
            try
            {
                var clientes = await _externalApiService.GetClientesAsync();
                var clienteExistente = clientes.FirstOrDefault(c => c.Id == oportunidad.ClienteId);
                if (clienteExistente == null)
                {
                    // cliente no encontrado -> 400 con mensaje claro
                    return BadRequest(new { mensaje = "El cliente especificado no existe en el sistema externo" });
                }

                // Enriquecer datos del cliente para mostrar (no necesarios para la inserción en BD)
                oportunidad.ClienteNombre = clienteExistente.ContactoId?.Name ?? string.Empty;
                oportunidad.ClienteEmail = clienteExistente.ContactoId?.Correo ?? string.Empty;
                oportunidad.ClienteTelefono = clienteExistente.ContactoId?.Telefono ?? string.Empty;
            }
            catch (Exception ex)
            {
                // Falló la validación externa -> 502 Bad Gateway (o 400 si prefieres)
                _logger.LogError(ex, "Error validando cliente externo para ClienteId {ClienteId}", oportunidad.ClienteId);
                return StatusCode(502, new { mensaje = "No se pudo validar el cliente en el servicio externo", detalle = ex.Message });
            }

            // Persistir con manejo de excepciones claro
            try
            {
                _context.Oportunidades.Add(oportunidad);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetPorId), new { id = oportunidad.Id }, oportunidad);
            }
            catch (DbUpdateException dbEx)
            {
                // DB error al insertar -> 500 con detalle mínimo y log para servidor
                _logger.LogError(dbEx, "Error al guardar Oportunidad (ClienteId={ClienteId}, Titulo={Titulo})", oportunidad.ClienteId, oportunidad.Titulo);
                return StatusCode(500, new { mensaje = "Error al guardar la oportunidad en la base de datos", detalle = dbEx.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado en Crear Oportunidad");
                return StatusCode(500, new { mensaje = "Error inesperado al crear la oportunidad", detalle = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Oportunidad>> GetPorId(int id)
        {
            var oportunidad = await _context.Oportunidades
                .Include(o => o.Tareas)
                .Include(o => o.Propuestas)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (oportunidad == null)
                return NotFound();

            return oportunidad;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Oportunidad>>> GetAll()
        {
            var oportunidades = await _context.Oportunidades
                .Include(o => o.Tareas)
                .Include(o => o.Propuestas)
                .ToListAsync();

            return oportunidades;
        }

        [HttpGet("por-cliente/{clienteId}")]
        public async Task<ActionResult<IEnumerable<Oportunidad>>> GetPorCliente(string clienteId)
        {
            var oportunidades = await _context.Oportunidades
                .Where(o => o.ClienteId == clienteId)
                .Include(o => o.Tareas)
                .Include(o => o.Propuestas)
                .ToListAsync();

            return oportunidades;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Actualizar(int id, [FromBody] Oportunidad oportunidad)
        {
            if (id != oportunidad.Id)
                return BadRequest();

            _context.Entry(oportunidad).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var oportunidad = await _context.Oportunidades.FindAsync(id);
            if (oportunidad == null)
                return NotFound();

            _context.Oportunidades.Remove(oportunidad);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}