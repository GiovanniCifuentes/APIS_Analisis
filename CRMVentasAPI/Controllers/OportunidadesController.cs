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

        public OportunidadesController(AppDbContext context, IExternalApiService externalApiService)
        {
            _context = context;
            _externalApiService = externalApiService;
        }

        [HttpPost]
        public async Task<ActionResult<Oportunidad>> Crear(Oportunidad oportunidad)
        {
            // Validar que el cliente existe en la API externa
            try
            {
                var clientes = await _externalApiService.GetClientesAsync();
                var clienteExistente = clientes.FirstOrDefault(c => c.Id == oportunidad.ClienteId);

                if (clienteExistente == null)
                    return BadRequest(new { mensaje = "El cliente especificado no existe en el sistema externo" });

                // Enriquecer datos del cliente para mostrar
                oportunidad.ClienteNombre = clienteExistente.ContactoId.Name;
                oportunidad.ClienteEmail = clienteExistente.ContactoId.Correo;
                oportunidad.ClienteTelefono = clienteExistente.ContactoId.Telefono;
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = $"Error al validar cliente: {ex.Message}" });
            }

            _context.Oportunidades.Add(oportunidad);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPorId), new { id = oportunidad.Id }, oportunidad);
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