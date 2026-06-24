using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TaskMaster.MVC.DTOs;
using TaskMaster.MVC.Services;

namespace TaskMaster.MVC.Controllers
{
    [Authorize]
    public class ProjectTasksController : Controller
    {
        private readonly IApiService _apiService;

        public ProjectTasksController(IApiService apiService)
        {
            _apiService = apiService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var tasks = await _apiService.GetAsync<List<ProjectTaskDto>>("api/ProjectTasks");
            return View(tasks ?? new List<ProjectTaskDto>());
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await FillViewBags();
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create(ProjectTaskDto taskDto)
        {
            if (ModelState.IsValid)
            {
                await _apiService.PostAsync<ProjectTaskDto, ProjectTaskDto>("api/ProjectTasks", taskDto);
                return RedirectToAction(nameof(Index));
            }

            await FillViewBags(taskDto.ProjectId, taskDto.DeveloperId);

            return View(taskDto);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var task = await _apiService.GetAsync<ProjectTaskDto>($"api/ProjectTasks/{id}");
            if (task is null)
            {
                return NotFound();
            }

            await FillViewBags(task.ProjectId, task.DeveloperId);

            return View(task);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Edit(int id, ProjectTaskDto taskDto)
        {
            if (id != taskDto.ProjectTaskId)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                await _apiService.PutAsync<ProjectTaskDto>($"api/ProjectTasks/{id}", taskDto);
                return RedirectToAction(nameof(Index));
            }

            await FillViewBags(taskDto.ProjectId, taskDto.DeveloperId);

            return View(taskDto);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _apiService.DeleteAsync($"api/ProjectTasks/{id}");
            return RedirectToAction(nameof(Index));
        }

        private async Task FillViewBags(int? projectId = null, int? developerId = null)
        {
            var projects = await _apiService.GetAsync<List<ProjectDto>>("api/Projects") ?? new List<ProjectDto>();
            var developers = await _apiService.GetAsync<List<DeveloperDto>>("api/Developers") ?? new List<DeveloperDto>();

            ViewBag.Projects = new SelectList(projects, "ProjectId", "Name", projectId);
            // DTO içerisindeki "FullName" özelliğini kullanarak listeliyoruz
            ViewBag.Developers = new SelectList(developers, "DeveloperId", "FullName", developerId);
        }
        
    }
}
