using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskMaster.MVC.DTOs;
using TaskMaster.MVC.Services;

namespace TaskMaster.MVC.Controllers
{
    [Authorize]
    public class ProjectsController : Controller
    {
        private readonly IApiService _apiService;

        public ProjectsController(IApiService apiService)
        {
            _apiService = apiService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var projects = await _apiService.GetAsync<List<ProjectDto>>("api/Projects");
            return View(projects ?? new List<ProjectDto>());
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create(ProjectDto project)
        {
            if (ModelState.IsValid)
            {
                await _apiService.PostAsync<ProjectDto, ProjectDto>("api/Projects", project);
                return RedirectToAction(nameof(Index));
            }

            return View(project);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var project = await _apiService.GetAsync<ProjectDto>($"api/Projects/{id}");

            if (project is null)
            {
                return NotFound();
            }

            return View(project);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Edit(int id, ProjectDto project)
        {
            if (id != project.ProjectId)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                await _apiService.PutAsync<ProjectDto>($"api/Projects/{id}", project);
                return RedirectToAction(nameof(Index));
            }

            return View(project);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _apiService.DeleteAsync($"api/Projects/{id}");
            return RedirectToAction(nameof(Index));
        }
    }
}
