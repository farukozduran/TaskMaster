using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using TaskMaster.API.DTOs;
using TaskMaster.API.Models;

namespace TaskMaster.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProjectsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProjectDto>>> GetProjects()
        {
            return await _context.Projects
                .Select(p => new ProjectDto
                {
                    ProjectId = p.ProjectId,
                    Name = p.Name,
                    StartDate = p.StartDate,
                    EndDate = p.EndDate
                })
                .ToListAsync();
        }

        [HttpGet("{id}")]

        public async Task<ActionResult<ProjectDto>> GetProjectById(int id)
        {
            var projectDto = await _context.Projects
                .Where(p => p.ProjectId == id)
                .Select(p => new ProjectDto
                {
                    ProjectId = p.ProjectId,
                    Name = p.Name,
                    StartDate = p.StartDate,
                    EndDate = p.EndDate
                })
                .FirstOrDefaultAsync();

            if (projectDto is null)
            {
                return NotFound(new { Message = "Proje bulunamadi."});
            }

            return projectDto;
        }

        [HttpPost]
        public async Task<ActionResult<ProjectDto>> AddProject(ProjectDto dto)
        {
            var project = new Project
            {
                Name = dto.Name,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate
            };

            _context.Projects.Add(project);
            await _context.SaveChangesAsync();

            dto.ProjectId = project.ProjectId;
            return CreatedAtAction(nameof(GetProjectById), new { id = project.ProjectId }, project);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProject(int id, ProjectDto dto)
        {
            if (id != dto.ProjectId)
            {
                return BadRequest(new { Message = "ID eslesmiyor." });
            }

            var project = await _context.Projects.FindAsync(id);
            if (project is null)
            {
                return NotFound(new { Message = "Proje bulunamadi." });
            }

            project.Name = dto.Name;
            project.StartDate = dto.StartDate;
            project.EndDate = dto.EndDate;

            _context.Entry(project).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProject(int id)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project is null)
            {
                return NotFound(new { Message = "Silinecek proje bulunamadi." });
            }

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
