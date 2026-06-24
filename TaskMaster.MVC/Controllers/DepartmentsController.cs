using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskMaster.MVC.Services;
using TaskMaster.MVC.DTOs;

namespace TaskMaster.MVC.Controllers
{
    public class DepartmentsController : Controller
    {
        private readonly IApiService _apiService;

        public DepartmentsController(IApiService apiService)
        {
            _apiService = apiService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var departments = await _apiService.GetAsync<List<DepartmentDto>>("api/Departments");
            return View(departments ?? new List<DepartmentDto>());
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create(DepartmentDto department)
        {
            if (ModelState.IsValid)
            {
                await _apiService.PostAsync<DepartmentDto, DepartmentDto>("api/Departments", department);
                return RedirectToAction(nameof(Index));
            }

            return View(department);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var department = await _apiService.GetAsync<DepartmentDto>($"api/Departments/{id}");

            if (department is null)
            {
                return NotFound();
            }

            return View(department);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Edit(int id, DepartmentDto department)
        {
            if (id != department.DepartmentId)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                await _apiService.PutAsync<DepartmentDto>($"api/Departments/{id}", department);
                return RedirectToAction(nameof(Index));
            }

            return View(department);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _apiService.DeleteAsync($"api/Departments/{id}");
            return RedirectToAction(nameof(Index));
        }
    }
}
