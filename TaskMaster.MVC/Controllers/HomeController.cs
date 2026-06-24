using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.Json;
using TaskMaster.MVC.DTOs;
using TaskMaster.MVC.Services;

namespace TaskMaster.MVC.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IApiService _apiService;

        public HomeController(IApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> Index()
        {
            var projects = await _apiService.GetAsync<List<ProjectDto>>("api/Projects");

            if (projects is null)
            {
                return RedirectToAction("Login", "Auth");
            }

            return View(projects);
        }
    }
}