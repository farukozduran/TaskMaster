using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskMaster.API.DTOs;
using TaskMaster.API.Models;

namespace TaskMaster.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class DevelopersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DevelopersController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DeveloperDto>>> GetDevelopers()
        {
            return await _context.Developers
                .Include(d => d.Department)
                .Select(d => new DeveloperDto
                {
                    DeveloperId = d.DeveloperId,
                    FirstName = d.FirstName,
                    LastName = d.LastName,
                    Title = d.Title,
                    DepartmentId = d.DepartmentId,
                    DepartmentName = d.Department.Name
                })
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DeveloperDto>> GetDeveloperById(int id)
        {
            var developerDto = await _context.Developers
                .Include(d => d.Department)
                .Where(d => d.DeveloperId == id)
                .Select(d => new DeveloperDto
                {
                    DeveloperId = d.DeveloperId,
                    FirstName = d.FirstName,
                    LastName = d.LastName,
                    Title = d.Title,
                    DepartmentId = d.DepartmentId,
                    DepartmentName = d.Department.Name
                })
                .FirstOrDefaultAsync();

            if (developerDto is null)
            {
                return NotFound(new { Message = "Gelistirici bulunamadi." });
            }

            return developerDto;
        }

        [HttpPost]
        public async Task<ActionResult<DeveloperDto>> AddDeveloper(DeveloperDto dto)
        {
            var department = await _context.Departments.FindAsync(dto.DepartmentId);

            if (department is null)
            {
                return BadRequest(new { Message = "Gecersiz Departman ID." });
            }

            var developer = new Developer
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Title = dto.Title,
                DepartmentId = dto.DepartmentId,
            };

            _context.Developers.Add(developer);
            await _context.SaveChangesAsync();

            dto.DeveloperId = developer.DeveloperId;
            dto.DepartmentName = department.Name;

            return CreatedAtAction(nameof(GetDeveloperById), new { id = developer.DeveloperId }, developer);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDeveloper(int id, DeveloperDto dto)
        {
            if (id != dto.DeveloperId)
            {
                return BadRequest(new { Message = "ID eslesmiyor." });
            }

            var developer = await _context.Developers.FindAsync(id);
            if (developer is null)
            {
                return NotFound(new { Message = "Gelistirici bulunamadi." });
            }

            var department = await _context.Departments.FindAsync(dto.DepartmentId);
            if (department is null)
            {
                return BadRequest(new { Message = "Gecersiz Departman ID" });
            }

            developer.FirstName = dto.FirstName;
            developer.LastName = dto.LastName;
            developer.Title = dto.Title;
            developer.DepartmentId = dto.DepartmentId;

            _context.Entry(developer).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDeveloper(int id)
        {
            var developer = await _context.Developers.FindAsync(id);
            if (developer is null)
            {
                return NotFound(new { Message = "Silinecek gelistirici bulunamadi." });
            }

            _context.Developers.Remove(developer);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
