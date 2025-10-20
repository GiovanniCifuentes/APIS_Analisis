using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CRMVentasAPI;
using CRMVentasAPI.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace CRMVentasAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TareasController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<TareasController> _logger;

        public TareasController(AppDbContext context, ILogger<TareasController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Tarea>>> GetTareas()
        {
            return await _context.Tareas
                .Include(t => t.Oportunidad)
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Tarea>> GetTarea(int id)
        {
            var tarea = await _context.Tareas
                .Include(t => t.Oportunidad)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (tarea == null)
                return NotFound();

            return tarea;
        }

        [HttpGet("por-oportunidad/{oportunidadId}")]
        public async Task<ActionResult<IEnumerable<Tarea>>> GetTareasPorOportunidad(int oportunidadId)
        {
            return await _context.Tareas
                .Where(t => t.OportunidadId == oportunidadId)
                .ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<Tarea>> CreateTarea([FromBody] CreateTareaDto dto)
        {
            // Validación DTO
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // ✅ Validación robusta de la oportunidad
            if (dto.OportunidadId != 0)
            {
                var oportunidadExiste = await _context.Oportunidades
                    .AnyAsync(o => o.Id == dto.OportunidadId);

                if (!oportunidadExiste)
                    return BadRequest(new { mensaje = $"La oportunidad con ID {dto.OportunidadId} no existe." });
            }
            else
            {
                return BadRequest(new { mensaje = "Debe especificar un ID de oportunidad válido." });
            }

            // ✅ Parseo seguro de fecha (acepta null/empty)
            DateTime? fechaVencimiento = null;
            if (!string.IsNullOrWhiteSpace(dto.FechaVencimiento))
            {
                if (DateTime.TryParse(dto.FechaVencimiento, out var parsed))
                    fechaVencimiento = parsed;
                else
                {
                    // intentar parseo ISO explícito
                    try
                    {
                        fechaVencimiento = System.Xml.XmlConvert.ToDateTime(
                            dto.FechaVencimiento,
                            System.Xml.XmlDateTimeSerializationMode.Utc
                        );
                    }
                    catch
                    {
                        return BadRequest(new { mensaje = "Formato de FechaVencimiento inválido. Use ISO yyyy-MM-dd o una fecha válida." });
                    }
                }
            }

            // ✅ Crear nueva tarea
            var tarea = new Tarea
            {
                Titulo = dto.Titulo,
                Descripcion = dto.Descripcion,
                FechaVencimiento = fechaVencimiento,
                Completada = dto.Completada,
                OportunidadId = dto.OportunidadId
            };

            try
            {
                _context.Tareas.Add(tarea);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Error guardando tarea en BD (CreateTarea)");
                return StatusCode(500, new { mensaje = "Error guardando la tarea en la base de datos", detalle = dbEx.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado en CreateTarea");
                return StatusCode(500, new { mensaje = "Error inesperado al crear la tarea", detalle = ex.Message });
            }

            return CreatedAtAction(nameof(GetTarea), new { id = tarea.Id }, tarea);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTarea(int id, Tarea tarea)
        {
            if (id != tarea.Id)
                return BadRequest();

            _context.Entry(tarea).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPut("{id}/completar")]
        public async Task<IActionResult> CompletarTarea(int id)
        {
            var tarea = await _context.Tareas.FindAsync(id);
            if (tarea == null)
                return NotFound();

            tarea.Completada = true;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTarea(int id)
        {
            var tarea = await _context.Tareas.FindAsync(id);
            if (tarea == null)
                return NotFound();

            _context.Tareas.Remove(tarea);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
