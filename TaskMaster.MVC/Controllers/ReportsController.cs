using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TaskMaster.MVC.DTOs;
using TaskMaster.MVC.Services;

namespace TaskMaster.MVC.Controllers
{
    [Authorize]
    public class ReportsController : Controller
    {
        private readonly IApiService _apiService;

        public ReportsController(IApiService apiService)
        {
            _apiService = apiService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var devs = await _apiService.GetAsync<List<DeveloperDto>>("api/Developers") ?? new List<DeveloperDto>();
            ViewBag.Developers = new SelectList(devs, "DeveloperId", "FullName");

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetTasksByDeveloper(int developerId)
        {
            var allTasks = await _apiService.GetAsync<List<ProjectTaskDto>>("api/ProjectTasks") ?? new List<ProjectTaskDto>();

            var filteredTasks = allTasks
                .Where(t => t.DeveloperId == developerId)
                .Select(t => new
                {
                    Title = t.Title,
                    ProjectName = t.ProjectName,
                    Status = t.Status
                })
                .ToList();

            return Json(filteredTasks);
        }
    }
}
