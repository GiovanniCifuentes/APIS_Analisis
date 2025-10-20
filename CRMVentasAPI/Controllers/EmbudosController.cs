using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CRMVentasAPI; // 🔹 Corregido: ya no se usa .Data
using CRMVentasAPI.Models; // 🔹 Añadido para reconocer la clase Embudo
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CRMVentasAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmbudosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public EmbudosController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Embudos
        // 🔹 Obtiene todos los embudos de la base de datos.
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Embudo>>> GetEmbudos()
        {
            return await _context.Embudos.ToListAsync();
        }

        // GET: api/Embudos/5
        // 🔹 Obtiene un embudo específico por su ID.
        [HttpGet("{id}")]
        public async Task<ActionResult<Embudo>> GetEmbudo(int id)
        {
            var embudo = await _context.Embudos.FindAsync(id);

            if (embudo == null)
            {
                return NotFound();
            }

            return embudo;
        }

        // POST: api/Embudos
        // 🔹 Crea un nuevo embudo en la base de datos.
        [HttpPost]
        public async Task<ActionResult<Embudo>> PostEmbudo([FromBody] Embudo embudo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Embudos.Add(embudo);
            await _context.SaveChangesAsync();

            // Devuelve una respuesta 201 Created con la ubicación del nuevo recurso.
            return CreatedAtAction(nameof(GetEmbudo), new { id = embudo.Id }, embudo);
        }

        // PUT: api/Embudos/5
        // 🔹 Actualiza un embudo existente.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEmbudo(int id, [FromBody] Embudo embudo)
        {
            if (id != embudo.Id)
            {
                return BadRequest("El ID del embudo no coincide.");
            }

            _context.Entry(embudo).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Embudos.Any(e => e.Id == id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent(); // Devuelve 204 No Content si fue exitoso.
        }

        // DELETE: api/Embudos/5
        // 🔹 Elimina un embudo de la base de datos.
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmbudo(int id)
        {
            var embudo = await _context.Embudos.FindAsync(id);
            if (embudo == null)
            {
                return NotFound();
            }

            _context.Embudos.Remove(embudo);
            await _context.SaveChangesAsync();

            return NoContent(); // Devuelve 204 No Content si fue exitoso.
        }
    }
}
