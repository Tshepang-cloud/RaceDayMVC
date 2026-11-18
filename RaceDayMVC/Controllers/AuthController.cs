using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using RacedayMvc.Models;
using RacedayMvc.Services;

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
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            var loginData = new { Email = email, Password = password };
            var response = await _apiService.PostAsync("api/auth/login", JsonContent.Create(loginData));

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<LoginResponse>();

                if (result != null && result.User != null)
                {
                    HttpContext.Session.SetString("UserRole", result.User.Role);
                    HttpContext.Session.SetString("UserName", result.User.FullName);
                    HttpContext.Session.SetInt32("UserId", result.User.UserId);

                    if (result.User.Role == "Organiser")
                        return RedirectToAction("Dashboard", "Organiser");
                    else
                        return RedirectToAction("Index", "Participant");
                }
            }

            var errorDetails = await response.Content.ReadAsStringAsync();
            ViewBag.Error = $"Login failed: {errorDetails}";
            return View();
        }

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(string name, string email, string password, string role)
        {
            var regData = new { FullName = name, Email = email, Password = password, Role = role };
            var response = await _apiService.PostAsync("api/auth/register", JsonContent.Create(regData));

            if (response.IsSuccessStatusCode)
                return RedirectToAction("Login");

            var errorDetails = await response.Content.ReadAsStringAsync();
            ViewBag.Error = $"Registration failed. API says: {errorDetails}";
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}