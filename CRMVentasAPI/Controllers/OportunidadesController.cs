using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CRMVentasAPI;
using CRMVentasAPI.Models;
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

        public OportunidadesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult<Oportunidad>> Crear(Oportunidad oportunidad)
        {
            _context.Oportunidades.Add(oportunidad);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetPorId), new { id = oportunidad.Id }, oportunidad);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Oportunidad>> GetPorId(int id)
        {
            var oportunidad = await _context.Oportunidades
                .Include(o => o.Cliente)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (oportunidad == null)
                return NotFound(); // 🔹 Corregido para no causar advertencia

            return oportunidad;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Oportunidad>>> GetAll()
        {
            return await _context.Oportunidades.Include(o => o.Cliente).ToListAsync();
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
    }
}