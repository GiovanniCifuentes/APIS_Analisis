using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CRMVentasAPI; // 🔹 Corregido
using CRMVentasAPI.Models; // 🔹 Añadido
using ClosedXML.Excel;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CRMVentasAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AnalyticsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AnalyticsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("ExportarExcel")]
        public async Task<IActionResult> ExportarExcel([FromQuery] string? estado, [FromQuery] DateTime? fechaInicio, [FromQuery] DateTime? fechaFin)
        {
            var query = _context.Oportunidades.AsQueryable();

            if (!string.IsNullOrEmpty(estado))
                query = query.Where(o => o.Estado == estado);

            if (fechaInicio.HasValue)
                query = query.Where(o => o.FechaCierre >= fechaInicio.Value);

            if (fechaFin.HasValue)
                query = query.Where(o => o.FechaCierre <= fechaFin.Value);

            var oportunidades = await query.ToListAsync();

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Oportunidades");

            worksheet.Cell(1, 1).Value = "ID";
            worksheet.Cell(1, 2).Value = "Titulo";
            worksheet.Cell(1, 3).Value = "Estado";
            worksheet.Cell(1, 4).Value = "ValorEstimado";
            worksheet.Cell(1, 5).Value = "FechaCierre";
            worksheet.Cell(1, 6).Value = "Vendedor";

            for (int i = 0; i < oportunidades.Count; i++)
            {
                var o = oportunidades[i];
                worksheet.Cell(i + 2, 1).Value = o.Id;
                worksheet.Cell(i + 2, 2).Value = o.Titulo ?? "";
                worksheet.Cell(i + 2, 3).Value = o.Estado ?? "";
                worksheet.Cell(i + 2, 4).Value = o.ValorEstimado;
                worksheet.Cell(i + 2, 5).Value = o.FechaCierre?.ToString("yyyy-MM-dd") ?? "";
                worksheet.Cell(i + 2, 6).Value = o.Vendedor ?? "";
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Seek(0, SeekOrigin.Begin);

            return File(stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "prevision_ventas.xlsx");
        }
    }
}


