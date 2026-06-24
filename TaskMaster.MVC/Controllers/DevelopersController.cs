using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TaskMaster.MVC.DTOs;
using TaskMaster.MVC.Services;

namespace TaskMaster.MVC.Controllers
{
    [Authorize]
    public class DevelopersController : Controller
    {
        private readonly IApiService _apiService;

        public DevelopersController(IApiService apiService)
        {
            _apiService = apiService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var devs = await _apiService.GetAsync<List<DeveloperDto>>("api/Developers");
            return View(devs ?? new List<DeveloperDto>());
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var departments = await _apiService.GetAsync<List<DepartmentDto>>("api/Departments");

            ViewBag.Departments = new SelectList(departments, "DepartmentId", "Name");

            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create(DeveloperDto dev)
        {
            if (ModelState.IsValid)
            {
                await _apiService.PostAsync<DeveloperDto, DeveloperDto>("api/Developers", dev);
                return RedirectToAction(nameof(Index));
            }

            var departments = await _apiService.GetAsync<List<DepartmentDto>>("api/Departments");
            ViewBag.Departments = new SelectList(departments, "DepartmentId", "Name", dev.DepartmentId);

            return View(dev);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var dev = await _apiService.GetAsync<DeveloperDto>($"api/Developers/{id}");

            if (dev is null)
            {
                return NotFound();
            }

            var departments = await _apiService.GetAsync<List<DepartmentDto>>("api/Departments");

            ViewBag.Departments = new SelectList(departments, "DepartmentId", "Name");

            return View(dev);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Edit(int id, DeveloperDto dev)
        {
            if (id != dev.DeveloperId)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                await _apiService.PutAsync<DeveloperDto>($"api/Developers/{id}", dev);
                return RedirectToAction(nameof(Index));
            }

            var departments = await _apiService.GetAsync<List<DepartmentDto>>("api/Departments");
            ViewBag.Departments = new SelectList(departments, "DepartmentId", "Name", dev.DepartmentId);

            return View(dev);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _apiService.DeleteAsync($"api/Developers/{id}");
            return RedirectToAction(nameof(Index));
        }
    }
}
