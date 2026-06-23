using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        public async Task<ActionResult<IEnumerable<Developer>>> GetDevelopers()
        {
            return await _context.Developers
                .Include(d => d.Department)
                .Include(d => d.Tasks)
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Developer>> GetDeveloperById(int id)
        {
            var developer = await _context.Developers
                .Include(d => d.Department)
                .Include(d => d.Tasks)
                .FirstOrDefaultAsync(d => d.DepartmentId == id);

            if (developer is null)
            {
                return NotFound(new { Message = "Gelistirici bulunamadi." });
            }

            return developer;
        }

        [HttpPost]
        public async Task<ActionResult<Developer>> AddDeveloper(Developer developer)
        {
            var departmentExists = await _context.Departments.AnyAsync(d => d.DepartmentId == developer.DepartmentId);

            if (!departmentExists)
            {
                return BadRequest(new { Message = "Gecersiz Departman ID." });
            }

            _context.Developers.Add(developer);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetDeveloperById), new { id = developer.DeveloperId }, developer);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDeveloper(int id, Developer developer)
        {
            if (id != developer.DeveloperId)
            {
                return BadRequest(new { Message = "ID eslesmiyor." });
            }

            var departmentExists = await _context.Departments.AnyAsync(d => d.DepartmentId == developer.DepartmentId);
            if (!departmentExists)
            {
                return BadRequest(new { Message = "Gecersiz Departman ID" });
            }

            _context.Entry(developer).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Developers.Any(e => e.DeveloperId == id))
                    return NotFound(new { Message = "Gelistirici bulunamadi." });
                else
                    throw;
            }

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
