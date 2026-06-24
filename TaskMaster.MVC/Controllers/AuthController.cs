using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using TaskMaster.MVC.Models;
using System.IdentityModel.Tokens.Jwt;

namespace TaskMaster.MVC.Controllers
{
    public class AuthController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public AuthController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var client = _httpClientFactory.CreateClient("ApiClient");

            var jsonContent = JsonSerializer.Serialize(model);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("api/Auth/Login", content);

            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(
                    responseString,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );

                if(tokenResponse != null && !string.IsNullOrEmpty(tokenResponse.Token))
                {
                    var handler = new JwtSecurityTokenHandler();
                    var jwtToken = handler.ReadJwtToken(tokenResponse.Token);

                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, model.Username)
                    };

                    var roleClaims = jwtToken.Claims.Where(c => c.Type == ClaimTypes.Role || c.Type == "role");
                    claims.AddRange(roleClaims);

                    var claimsIdentity = new ClaimsIdentity(claims, "TaskMasterCookie");

                    var authProperties = new AuthenticationProperties();
                    authProperties.StoreTokens(new List<AuthenticationToken>
                    {
                        new AuthenticationToken{ Name = "jwt", Value = tokenResponse.Token}
                    });

                    await HttpContext.SignInAsync(
                        "TaskMasterCookie",
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties
                    );
                    return RedirectToAction("Index", "Home");
                }
            }
            ModelState.AddModelError(string.Empty, "Invalid username or password");
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("TaskMasterCookie");
            return RedirectToAction("Login", "Auth");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var client = _httpClientFactory.CreateClient("ApiClient");

            // API'nin beklediği RegisterDto yapısına uygun anonim nesne oluşturuyoruz
            var jsonContent = JsonSerializer.Serialize(new
            {
                Username = model.Username,
                Email = model.Email,
                Password = model.Password,
                FullName = model.FullName
            });
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("api/Auth/Register", content);

            if (response.IsSuccessStatusCode)
            {
                // Başarılı olursa Login ekranına yönlendirip mesaj verelim
                TempData["SuccessMessage"] = "Registration successful! You can now log in.";
                return RedirectToAction("Login");
            }

            ModelState.AddModelError(string.Empty, "Registration failed. Username or Email might already be taken.");
            return View(model);
        }
    }
}
