using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CRMVentasAPI;
using CRMVentasAPI.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace CRMVentasAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PropuestasController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PropuestasController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Propuesta>>> GetAll()
        {
            return Ok(await _context.Propuestas.AsNoTracking().ToListAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Propuesta>> GetById(int id)
        {
            var propuesta = await _context.Propuestas.FindAsync(id);
            if (propuesta == null)
                return NotFound(new { mensaje = $"No se encontró la propuesta con ID {id}" });

            return Ok(propuesta);
        }

        [HttpPost]
        public async Task<ActionResult<Propuesta>> Create(Propuesta propuesta)
        {
            propuesta.FechaCreacion = DateTime.UtcNow;
            _context.Propuestas.Add(propuesta);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = propuesta.Id }, propuesta);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Propuesta propuesta)
        {
            if (id != propuesta.Id)
                return BadRequest(new { mensaje = "El ID de la propuesta no coincide" });

            _context.Entry(propuesta).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var propuesta = await _context.Propuestas.FindAsync(id);
            if (propuesta == null)
                return NotFound(new { mensaje = $"No existe la propuesta con ID {id}" });

            _context.Propuestas.Remove(propuesta);
            await _context.SaveChangesAsync();
            return Ok(new { mensaje = "Propuesta eliminada correctamente" });
        }
    }
}
