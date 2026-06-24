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
    public class ProjectTasksController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProjectTasksController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProjectTaskDto>>> GetTasks()
        {
            return await _context.Tasks
                .Include(t => t.Project)
                .Include(t => t.Developer)
                .Select(t => new ProjectTaskDto
                {
                    ProjectTaskId = t.ProjectTaskId,
                    Title = t.Title,
                    Description = t.Description,
                    Status = t.Status,
                    ProjectId = t.ProjectId,
                    ProjectName = t.Project.Name,
                    DeveloperId = t.DeveloperId,
                    DeveloperFullName = t.Developer != null ? $"{t.Developer.FirstName} {t.Developer.LastName}" : null
                })
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProjectTaskDto>> GetTaskById(int id)
        {
            var taskDto = await _context.Tasks
                .Include(t => t.Project)
                .Include(t => t.Developer)
                .Where(t => t.ProjectTaskId == id)
                .Select(t => new ProjectTaskDto
                {
                    ProjectTaskId = t.ProjectTaskId,
                    Title = t.Title,
                    Description = t.Description,
                    Status = t.Status,
                    ProjectId = t.ProjectId,
                    ProjectName = t.Project.Name,
                    DeveloperId = t.DeveloperId,
                    DeveloperFullName = t.Developer != null ? $"{t.Developer.FirstName} {t.Developer.LastName}" : null
                })
                .FirstOrDefaultAsync();

            if (taskDto == null)
            {
                return NotFound();
            }

            return taskDto;
        }

        [HttpPost]
        public async Task<ActionResult<ProjectTaskDto>> AddTask(ProjectTaskDto dto)
        {
            var projectExists = await _context.Projects.AnyAsync(p => p.ProjectId == dto.ProjectId);
            if (!projectExists)
            {
                return BadRequest(new { Message = "Geçersiz Proje ID." });
            }

            if (dto.DeveloperId.HasValue)
            {
                var developerExists = await _context.Developers.AnyAsync(d => d.DeveloperId == dto.DeveloperId.Value);
                if (!developerExists)
                {
                    return BadRequest(new { Message = "Geçersiz Geliştirici ID." });
                }
            }

            var task = new ProjectTask
            {
                Title = dto.Title,
                Description = dto.Description,
                Status = dto.Status,
                ProjectId = dto.ProjectId,
                DeveloperId = dto.DeveloperId
            };

            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();

            dto.ProjectTaskId = task.ProjectTaskId;
            return CreatedAtAction(nameof(GetTaskById), new { id = task.ProjectTaskId }, dto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(int id, ProjectTaskDto dto)
        {
            if (id != dto.ProjectTaskId)
            {
                return BadRequest();
            }

            var task = await _context.Tasks.FindAsync(id);
            if (task == null)
            {
                return NotFound();
            }

            task.Title = dto.Title;
            task.Description = dto.Description;
            task.Status = dto.Status;
            task.ProjectId = dto.ProjectId;
            task.DeveloperId = dto.DeveloperId;

            _context.Entry(task).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var projectTask = await _context.Tasks.FindAsync(id);
            if (projectTask == null)
            {
                return NotFound();
            }

            _context.Tasks.Remove(projectTask);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
