using CRMVentasAPI.Models;
using CRMVentasAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace CRMVentasAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CondicionesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;
        private readonly IExternalApiService _externalApiService;

        public CondicionesController(AppDbContext context, IConfiguration config, IExternalApiService externalApiService)
        {
            _context = context;
            _config = config;
            _externalApiService = externalApiService;
        }

        // GET: api/Condiciones
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CondicionComercial>>> GetAll()
        {
            return Ok(await _context.CondicionesComerciales.AsNoTracking().ToListAsync());
        }

        // GET api/Condiciones/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<CondicionComercial>> GetById(int id)
        {
            var cond = await _context.CondicionesComerciales.FindAsync(id);
            if (cond == null) return NotFound(new { mensaje = "Condición no encontrada" });
            return Ok(cond);
        }

        // POST api/Condiciones
        [HttpPost]
        public async Task<ActionResult<CondicionComercial>> Create(CondicionComercial condicion)
        {
            condicion.FechaCreacion = DateTime.Now;
            condicion.UltimaActualizacion = DateTime.Now;

            _context.CondicionesComerciales.Add(condicion);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = condicion.Id }, condicion);
        }

        // PUT api/Condiciones/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, CondicionComercial condicion)
        {
            if (id != condicion.Id) return BadRequest(new { mensaje = "Id no coincide" });

            condicion.UltimaActualizacion = DateTime.Now;
            _context.Entry(condicion).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.CondicionesComerciales.Any(c => c.Id == id))
                    return NotFound(new { mensaje = "Condición no encontrada" });

                throw;
            }
        }

        // DELETE api/Condiciones/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var cond = await _context.CondicionesComerciales.FindAsync(id);
            if (cond == null) return NotFound(new { mensaje = "No existe la condición" });

            _context.CondicionesComerciales.Remove(cond);
            await _context.SaveChangesAsync();
            return Ok(new { mensaje = "Condición eliminada" });
        }

        // POST api/Condiciones/{id}/asignar?oportunidadId=5&clienteId=abc123
        [HttpPost("{id}/asignar")]
        public async Task<IActionResult> AsignarA(int id, [FromQuery] int? oportunidadId, [FromQuery] string? clienteId)
        {
            var cond = await _context.CondicionesComerciales.FindAsync(id);
            if (cond == null) return NotFound(new { mensaje = "Condición no encontrada" });

            // Validar que la oportunidad existe si se proporciona
            if (oportunidadId.HasValue)
            {
                var oportunidad = await _context.Oportunidades.FindAsync(oportunidadId.Value);
                if (oportunidad == null)
                    return BadRequest(new { mensaje = "La oportunidad especificada no existe" });
                cond.OportunidadId = oportunidadId;
            }

            // Validar que el cliente existe en la API externa si se proporciona
            if (!string.IsNullOrEmpty(clienteId))
            {
                try
                {
                    var clientes = await _externalApiService.GetClientesAsync();
                    var clienteExistente = clientes.FirstOrDefault(c => c.Id == clienteId);
                    if (clienteExistente == null)
                        return BadRequest(new { mensaje = "El cliente especificado no existe en el sistema externo" });

                    cond.ClienteId = clienteId;
                }
                catch (Exception ex)
                {
                    return BadRequest(new { mensaje = $"Error al validar cliente: {ex.Message}" });
                }
            }

            cond.UltimaActualizacion = DateTime.Now;
            await _context.SaveChangesAsync();
            return Ok(cond);
        }

        // DTO para el historial detallado
        public class HistorialDescuentoDto
        {
            public int Id { get; set; }
            public string? ClienteId { get; set; }
            public string? ClienteNombre { get; set; }
            public int? OportunidadId { get; set; }
            public string? OportunidadNombre { get; set; }
            public int? PropuestaId { get; set; }
            public string? PropuestaTitulo { get; set; }
            public decimal? PropuestaMonto { get; set; }
            public decimal PrecioOriginal { get; set; }
            public double DescuentoPorcentual { get; set; }
            public decimal PrecioFinal { get; set; }
            public string UsuarioAplicacion { get; set; } = string.Empty;
            public DateTime FechaAplicacion { get; set; }
            public string Comentarios { get; set; } = string.Empty;
            public int? CondicionComercialId { get; set; }
        }

        // GET api/Condiciones/historial-detallado?clienteId=abc123&oportunidadId=12
        [HttpGet("historial-detallado")]
        public async Task<ActionResult<IEnumerable<HistorialDescuentoDto>>> GetHistorialDetallado([FromQuery] string? clienteId, [FromQuery] int? oportunidadId)
        {
            var query = _context.DescuentosAplicados.AsQueryable();

            if (!string.IsNullOrEmpty(clienteId))
                query = query.Where(d => d.ClienteId == clienteId);
            if (oportunidadId.HasValue)
                query = query.Where(d => d.OportunidadId == oportunidadId.Value);

            var data = await query
                .OrderByDescending(d => d.FechaAplicacion)
                .Take(500)
                .Select(d => new HistorialDescuentoDto
                {
                    Id = d.Id,
                    ClienteId = d.ClienteId,
                    OportunidadId = d.OportunidadId,
                    OportunidadNombre = d.OportunidadId != null
                        ? _context.Oportunidades.Where(o => o.Id == d.OportunidadId).Select(o => o.Titulo).FirstOrDefault()
                        : null,
                    PropuestaId = d.PropuestaId,
                    PropuestaTitulo = d.PropuestaId != null
                        ? _context.Propuestas.Where(p => p.Id == d.PropuestaId).Select(p => p.Titulo).FirstOrDefault()
                        : null,
                    PropuestaMonto = d.PropuestaId != null
                        ? _context.Propuestas.Where(p => p.Id == d.PropuestaId).Select(p => p.Monto).FirstOrDefault()
                        : (decimal?)null,
                    PrecioOriginal = d.PrecioOriginal,
                    DescuentoPorcentual = d.DescuentoPorcentual,
                    PrecioFinal = d.PrecioFinal,
                    UsuarioAplicacion = d.UsuarioAplicacion,
                    FechaAplicacion = d.FechaAplicacion,
                    Comentarios = d.Comentarios,
                    CondicionComercialId = d.CondicionComercialId
                })
                .ToListAsync();

            // Enriquecer con datos de clientes externos
            try
            {
                var clientesExternos = await _externalApiService.GetClientesAsync();
                foreach (var item in data)
                {
                    if (!string.IsNullOrEmpty(item.ClienteId))
                    {
                        var clienteExterno = clientesExternos.FirstOrDefault(c => c.Id == item.ClienteId);
                        if (clienteExterno != null)
                        {
                            item.ClienteNombre = clienteExterno.ContactoId.Name;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Si falla la API externa, continuar sin los nombres de clientes
                Console.WriteLine($"Error obteniendo datos de clientes: {ex.Message}");
            }

            return Ok(data);
        }

        // GET api/Condiciones/historial-simplificado
        [HttpGet("historial-simplificado")]
        public async Task<ActionResult<IEnumerable<object>>> GetHistorialSimplificado()
        {
            try
            {
                var clientesExternos = await _externalApiService.GetClientesAsync();

                // Corregimos las advertencias de nulabilidad
                var descuentosPorCliente = await _context.DescuentosAplicados
                    .Where(d => !string.IsNullOrEmpty(d.ClienteId))
                    .GroupBy(d => d.ClienteId!)
                    .Select(g => new
                    {
                        ClienteId = g.Key,
                        CantidadDescuentos = g.Count()
                    })
                    .ToDictionaryAsync(g => g.ClienteId, g => g.CantidadDescuentos);

                var resultado = clientesExternos.Select(c => new
                {
                    Id = c.Id,
                    Nombre = c.ContactoId.Name,
                    CantidadDescuentos = descuentosPorCliente.ContainsKey(c.Id) ? descuentosPorCliente[c.Id] : 0
                }).ToList();

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = $"Error al obtener historial simplificado: {ex.Message}" });
            }
        }

        // POST api/Condiciones/aplicar
        [HttpPost("aplicar")]
        public async Task<IActionResult> AplicarDescuento([FromBody] AplicacionDescuentoDto dto)
        {
            // 1) política global
            double maxGlobal = _config.GetValue<double>("DiscountPolicy:MaxDiscountPercentGlobal", 25.0);

            // 2) buscar condición (oportunidad primero, luego cliente)
            CondicionComercial? condicion = null;
            if (dto.OportunidadId.HasValue)
            {
                condicion = await _context.CondicionesComerciales
                    .Where(c => c.OportunidadId == dto.OportunidadId && c.Activa)
                    .OrderByDescending(c => c.FechaCreacion)
                    .FirstOrDefaultAsync();
            }

            if (condicion == null && !string.IsNullOrEmpty(dto.ClienteId))
            {
                condicion = await _context.CondicionesComerciales
                    .Where(c => c.ClienteId == dto.ClienteId && c.Activa)
                    .OrderByDescending(c => c.FechaCreacion)
                    .FirstOrDefaultAsync();
            }

            double maxPermitido = maxGlobal;
            string fuente = "Política global";
            if (condicion != null)
            {
                maxPermitido = Math.Max(maxPermitido, condicion.DescuentoPorcentual);
                fuente = $"Condición [{condicion.Id}]";
            }

            // Validaciones
            if (dto.DescuentoSolicitado < 0)
                return BadRequest(new { mensaje = "El descuento no puede ser negativo" });

            if (dto.DescuentoSolicitado > 100)
                return BadRequest(new { mensaje = "El descuento no puede superar 100%" });

            if (dto.DescuentoSolicitado > maxPermitido)
            {
                return BadRequest(new
                {
                    mensaje = "Descuento excede el máximo permitido",
                    descuentoSolicitado = dto.DescuentoSolicitado,
                    maxPermitido,
                    fuente
                });
            }

            // Calculo precio final
            decimal precioFinal = dto.PrecioOriginal - (dto.PrecioOriginal * (decimal)(dto.DescuentoSolicitado / 100.0));
            if (!_config.GetValue<bool>("DiscountPolicy:AllowNegativePrices", false) && precioFinal < 0)
            {
                return BadRequest(new { mensaje = "El precio final no puede ser negativo" });
            }

            var registro = new DescuentoAplicado
            {
                ClienteId = dto.ClienteId,
                OportunidadId = dto.OportunidadId,
                PropuestaId = dto.PropuestaId,
                CondicionComercialId = condicion?.Id,
                PrecioOriginal = dto.PrecioOriginal,
                DescuentoPorcentual = dto.DescuentoSolicitado,
                PrecioFinal = Math.Round(precioFinal, 2),
                UsuarioAplicacion = dto.UsuarioAplicacion ?? "sistema",
                FechaAplicacion = DateTime.Now,
                Comentarios = dto.Comentarios ?? $"Aplicado vía {fuente}"
            };

            _context.DescuentosAplicados.Add(registro);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensaje = "Descuento aplicado correctamente",
                registro.Id,
                registro.PrecioOriginal,
                registro.DescuentoPorcentual,
                registro.PrecioFinal,
                registro.FechaAplicacion,
                fuente
            });
        }

        // GET api/Condiciones/resumen-propuesta?propuestaId=2&clienteId=abc123&oportunidadId=1
        [HttpGet("resumen-propuesta")]
        public async Task<IActionResult> ResumenPropuesta([FromQuery] int propuestaId, [FromQuery] string? clienteId, [FromQuery] int? oportunidadId)
        {
            var propuesta = await _context.Propuestas.FindAsync(propuestaId);
            decimal precioBase = propuesta != null ? propuesta.Monto : 0m;

            CondicionComercial? condicion = null;
            if (oportunidadId.HasValue)
            {
                condicion = await _context.CondicionesComerciales
                    .Where(c => c.OportunidadId == oportunidadId && c.Activa && (c.FechaFin == null || c.FechaFin > DateTime.Now))
                    .OrderByDescending(c => c.FechaCreacion).FirstOrDefaultAsync();
            }

            if (condicion == null && !string.IsNullOrEmpty(clienteId))
            {
                condicion = await _context.CondicionesComerciales
                    .Where(c => c.ClienteId == clienteId && c.Activa && (c.FechaFin == null || c.FechaFin > DateTime.Now))
                    .OrderByDescending(c => c.FechaCreacion).FirstOrDefaultAsync();
            }

            double maxGlobal = _config.GetValue<double>("DiscountPolicy:MaxDiscountPercentGlobal", 25.0);

            return Ok(new
            {
                propuestaId,
                precioBase,
                condicionAplicable = condicion != null ? new
                {
                    condicion.Id,
                    condicion.DescuentoPorcentual,
                    condicion.PrecioEspecial,
                    condicion.FechaInicio,
                    condicion.FechaFin,
                    condicion.Notas
                } : null,
                politicaGlobalMaxima = maxGlobal
            });
        }
    }
}