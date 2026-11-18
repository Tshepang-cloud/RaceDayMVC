using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using RacedayMvc.Services; // <--- THIS LINE FIXES THE RED SQUIGGLY LINES

namespace RacedayMvc.Controllers
{
    public class AuthController : Controller
    {
        private readonly IApiService _apiService;

        public AuthController(IApiService apiService)
        {
            _apiService = apiService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            var loginData = new { Email = email, Password = password };
            var response = await _apiService.PostAsync("api/auth/login", JsonContent.Create(loginData));

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<LoginResponse>();

                if (result != null)
                {
                    HttpContext.Session.SetString("UserRole", result.Role);
                    HttpContext.Session.SetString("UserName", result.Name);
                    HttpContext.Session.SetInt32("UserId", result.UserId);

                    if (result.Role == "Organiser")
                        return RedirectToAction("Dashboard", "Organiser");
                    else
                        return RedirectToAction("Index", "Participant");
                }
            }

            ViewBag.Error = "Invalid login credentials.";
            return View();
        }

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(string name, string email, string password, string role)
        {
            var regData = new { Name = name, Email = email, Password = password, Role = role };
            var response = await _apiService.PostAsync("api/auth/register", JsonContent.Create(regData));

            if (response.IsSuccessStatusCode)
                return RedirectToAction("Login");

            ViewBag.Error = "Registration failed.";
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }

    // DTO to match your API's login response
    public class LoginResponse
    {
        public int UserId { get; set; }
        public string Name { get; set; } = string.Empty; // Fixed warning
        public string Role { get; set; } = string.Empty; // Fixed warning
    }
}