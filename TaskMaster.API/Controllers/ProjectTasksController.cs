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
    public class ProjectTasksController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProjectTasksController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProjectTask>>> GetTasks()
        {
            return await _context.Tasks
                .Include(t => t.Project)
                .Include(t => t.Developer)
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProjectTask>> GetTask(int id)
        {
            var projectTask = await _context.Tasks
                .Include(t => t.Project)
                .Include(t => t.Developer)
                .FirstOrDefaultAsync(t => t.ProjectTaskId == id);

            if (projectTask == null)
            {
                return NotFound();
            }

            return projectTask;
        }

        [HttpPost]
        public async Task<ActionResult<ProjectTask>> AddTask(ProjectTask projectTask)
        {
            _context.Tasks.Add(projectTask);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTask), new { id = projectTask.ProjectTaskId }, projectTask);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(int id, ProjectTask projectTask)
        {
            if (id != projectTask.ProjectTaskId)
            {
                return BadRequest();
            }

            _context.Entry(projectTask).State = EntityState.Modified;
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
