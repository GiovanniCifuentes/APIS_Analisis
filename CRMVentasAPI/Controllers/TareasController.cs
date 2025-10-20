// Controllers/TareasController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CRMVentasAPI;
using CRMVentasAPI.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CRMVentasAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TareasController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TareasController(AppDbContext context)
        {
            _context = context;
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
        public async Task<ActionResult<Tarea>> CreateTarea(Tarea tarea)
        {
            // Validar que la oportunidad existe
            var oportunidad = await _context.Oportunidades.FindAsync(tarea.OportunidadId);
            if (oportunidad == null)
                return BadRequest(new { mensaje = "La oportunidad especificada no existe" });

            _context.Tareas.Add(tarea);
            await _context.SaveChangesAsync();

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
