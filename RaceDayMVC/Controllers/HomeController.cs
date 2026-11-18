using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

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
        // Create payload for API
        var loginData = new { Email = email, Password = password };

        // Send to API (Replace 'api/auth/login' with your actual endpoint)
        var response = await _apiService.PostAsync("api/auth/login", JsonContent.Create(loginData));

        if (response.IsSuccessStatusCode)
        {
            // Assuming your API returns an object with UserId, Name, and Role
            var result = await response.Content.ReadFromJsonAsync<LoginResponse>();

            // SET SESSION VARIABLES
            HttpContext.Session.SetString("UserRole", result.Role);
            HttpContext.Session.SetString("UserName", result.Name);
            HttpContext.Session.SetInt32("UserId", result.UserId);

            // Redirect based on role
            if (result.Role == "Organiser")
                return RedirectToAction("Dashboard", "Organiser");
            else
                return RedirectToAction("Index", "Participant");
        }

        ViewBag.Error = "Invalid login credentials.";
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
    public string Name { get; set; }
    public string Role { get; set; }
}