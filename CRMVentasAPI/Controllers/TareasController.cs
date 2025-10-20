using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CRMVentasAPI;
using CRMVentasAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

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
        public async Task<ActionResult<IEnumerable<Tarea>>> GetAll()
        {
            return Ok(await _context.Tareas.AsNoTracking().ToListAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Tarea>> GetById(int id)
        {
            var tarea = await _context.Tareas.FindAsync(id);
            if (tarea == null) return NotFound(new { mensaje = $"No se encontró la tarea con ID {id}" });

            return Ok(tarea);
        }

        [HttpPost]
        public async Task<ActionResult<Tarea>> Create(Tarea tarea)
        {
            _context.Tareas.Add(tarea);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = tarea.Id }, tarea);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Tarea tarea)
        {
            if (id != tarea.Id) return BadRequest(new { mensaje = "El ID de la tarea no coincide" });
            _context.Entry(tarea).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var tarea = await _context.Tareas.FindAsync(id);
            if (tarea == null) return NotFound(new { mensaje = $"No existe la tarea con ID {id}" });
            _context.Tareas.Remove(tarea);
            await _context.SaveChangesAsync();
            return Ok(new { mensaje = "Tarea eliminada correctamente" });
        }
    }
}
