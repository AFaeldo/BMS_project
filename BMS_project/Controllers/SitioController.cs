using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BMS_project.Data;
using BMS_project.Models;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using BMS_project.Services;
using System.Security.Claims;

namespace BMS_project.Controllers
{
    [Authorize(Roles = "BarangaySk")]
    [Route("api/[controller]")]
    [ApiController]
    public class SitioController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ISystemLogService _systemLogService;

        public SitioController(ApplicationDbContext context, ISystemLogService systemLogService)
        {
            _context = context;
            _systemLogService = systemLogService;
        }

        private int? GetBarangayIdFromClaims()
        {
            var claim = User.Claims.FirstOrDefault(c => c.Type == "Barangay_ID");
            return claim != null && int.TryParse(claim.Value, out int id) ? id : (int?)null;
        }

        private int? GetCurrentUserId()
        {
             var claim = User.FindFirst(ClaimTypes.NameIdentifier);
             if (claim != null && int.TryParse(claim.Value, out int id)) return id;
             return null;
        }

        [HttpPost("Add")]
        public async Task<IActionResult> AddSitio([FromBody] SitioDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name)) return BadRequest("Sitio Name is required.");

            var barangayId = GetBarangayIdFromClaims();
            if (barangayId == null) return Unauthorized("User is not assigned to a Barangay.");

            var exists = await _context.Sitios.AnyAsync(s => s.Barangay_ID == barangayId && s.Sitio_Name == dto.Name);
            if (exists) return BadRequest("Sitio already exists in this Barangay.");

            var sitio = new Sitio
            {
                Sitio_Name = dto.Name,
                Barangay_ID = barangayId.Value
            };

            _context.Sitios.Add(sitio);
            await _context.SaveChangesAsync();

            // Log
            var userId = GetCurrentUserId();
            if (userId.HasValue)
            {
                await _systemLogService.LogAsync(userId.Value, "Add Sitio", $"Added Sitio: {dto.Name}", "Sitio", sitio.Sitio_ID);
            }

            return Ok(new { success = true, id = sitio.Sitio_ID, name = sitio.Sitio_Name });
        }

        [HttpGet("List")]
        public async Task<IActionResult> GetSitios()
        {
            var barangayId = GetBarangayIdFromClaims();
            if (barangayId == null) return Unauthorized();

            var sitios = await _context.Sitios
                .Where(s => s.Barangay_ID == barangayId)
                .OrderBy(s => s.Sitio_Name)
                .Select(s => new { id = s.Sitio_ID, name = s.Sitio_Name })
                .ToListAsync();

            return Ok(sitios);
        }
    }

    public class SitioDto
    {
        public string Name { get; set; }
    }
}
