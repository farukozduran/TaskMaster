using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.Json;
using TaskMaster.MVC.DTOs;

namespace TaskMaster.MVC.Controllers
{
    // Authorize etiketi sayesinde bu sayfaya sadece Token'ı olan (giriş yapmış) kişiler girebilir
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public HomeController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index()
        {
            // 1. Çerezin içine sakladığımız JWT'yi güvenli bir şekilde okuyoruz
            var token = await HttpContext.GetTokenAsync("TaskMasterCookie", "jwt");

            // 2. API Client'ı oluşturuyoruz (Program.cs'te yapılandırdığımız base adresi kullanır)
            var client = _httpClientFactory.CreateClient("ApiClient");

            // 3. Okuduğumuz Token'ı "Bearer <token>" formatında isteğin başlığına (Header) ekliyoruz
            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            // 4. API'den projeleri GET isteği ile çekiyoruz
            var response = await client.GetAsync("api/Projects");
            var projects = new List<ProjectDto>();

            if (response.IsSuccessStatusCode)
            {
                // İstek başarılıysa gelen JSON yanıtını C# nesnelerine (List<ProjectDto>) dönüştür
                var responseString = await response.Content.ReadAsStringAsync();
                projects = JsonSerializer.Deserialize<List<ProjectDto>>(
                    responseString,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                // Eğer API 401 Unauthorized dönerse (örneğin token süresi dolmuşsa), Login'e yönlendir
                return RedirectToAction("Login", "Auth");
            }

            // Verileri Index.cshtml sayfasına gönder
            return View(projects);
        }
    }
}