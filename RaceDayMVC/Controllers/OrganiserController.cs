using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using RacedayMvc.Models;
using RacedayMvc.Services;

namespace RacedayMvc.Controllers
{
    [RoleAuthorize("Organiser")]
    public class OrganiserController : Controller
    {
        private readonly IApiService _apiService;

        public OrganiserController(IApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> Dashboard()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var response = await _apiService.GetAsync($"api/events/organiser/{userId}");

            if (response.IsSuccessStatusCode)
            {
                var myEvents = await response.Content.ReadFromJsonAsync<List<EventViewModel>>();
                return View(myEvents ?? new List<EventViewModel>());
            }
            return View(new List<EventViewModel>());
        }
    }
}