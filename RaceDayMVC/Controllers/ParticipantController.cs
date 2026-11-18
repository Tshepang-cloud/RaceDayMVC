using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using RacedayMvc.Models;
using RacedayMvc.Services;

namespace RacedayMvc.Controllers
{
    [RoleAuthorize("Participant")]
    public class ParticipantController : Controller
    {
        private readonly IApiService _apiService;

        public ParticipantController(IApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> Index()
        {
            var response = await _apiService.GetAsync("api/events");
            if (response.IsSuccessStatusCode)
            {
                var events = await response.Content.ReadFromJsonAsync<List<EventViewModel>>();
                return View(events ?? new List<EventViewModel>());
            }
            return View(new List<EventViewModel>());
        }

        public async Task<IActionResult> MyEnrollments()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var response = await _apiService.GetAsync($"api/enrollments/participant/{userId}");
            if (response.IsSuccessStatusCode)
            {
                var enrollments = await response.Content.ReadFromJsonAsync<List<EventViewModel>>();
                return View(enrollments ?? new List<EventViewModel>());
            }
            return View(new List<EventViewModel>());
        }

        [HttpPost]
        public async Task<IActionResult> Enroll(int eventId, string category)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var enrollData = new { EventId = eventId, ParticipantId = userId, Category = category };
            var response = await _apiService.PostAsync("api/enrollments", JsonContent.Create(enrollData));

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Successfully enrolled!";
                return RedirectToAction("MyEnrollments");
            }
            TempData["Error"] = "Failed to enroll.";
            return RedirectToAction("Index");
        }
    }
}