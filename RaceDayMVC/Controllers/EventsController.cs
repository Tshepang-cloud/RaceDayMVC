using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using RacedayMvc.Models;
using RacedayMvc.Services;

namespace RacedayMvc.Controllers
{
    public class EventsController : Controller
    {
        private readonly IApiService _apiService;

        public EventsController(IApiService apiService)
        {
            _apiService = apiService;
        }

        [HttpGet]
        [RoleAuthorize("Organiser")]
        public IActionResult Create() => View();

        [HttpPost]
        [RoleAuthorize("Organiser")]
        public async Task<IActionResult> Create(EventViewModel model, IFormFile bannerImage)
        {
            if (ModelState.IsValid)
            {
                // 1. Verify the session actually has the UserId
                var userId = HttpContext.Session.GetInt32("UserId");
                if (userId == null)
                {
                    ViewBag.Error = "Session expired. Please log out and log in again.";
                    return View(model);
                }

                // 2. Build the exact payload your API expects
                // Note: Added a dummy Description in case your API requires it!
                var eventData = new
                {
                    EventName = model.Name,
                    EventDate = model.Date,
                    Location = model.Location,
                    OrganiserId = userId.Value,
                    Description = "Event created from MVC", // Add this just in case the API requires it
                    Distance = "10km" // Add this just in case the API requires it
                };

                var createResponse = await _apiService.PostAsync("api/events", JsonContent.Create(eventData));

                if (createResponse.IsSuccessStatusCode)
                {
                    var createdEvent = await createResponse.Content.ReadFromJsonAsync<EventResponse>();

                    if (bannerImage != null && bannerImage.Length > 0 && createdEvent != null)
                    {
                        using var content = new MultipartFormDataContent();
                        using var stream = bannerImage.OpenReadStream();
                        var fileContent = new StreamContent(stream);
                        fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(bannerImage.ContentType);
                        content.Add(fileContent, "file", bannerImage.FileName);

                        // Upload image
                        var uploadResponse = await _apiService.PostAsync($"api/events/{createdEvent.Id}/upload-banner", content);

                        if (!uploadResponse.IsSuccessStatusCode)
                        {
                            var uploadError = await uploadResponse.Content.ReadAsStringAsync();
                            ViewBag.Error = $"Event created, but image upload failed: {uploadError}";
                            return View(model);
                        }
                    }

                    return RedirectToAction("Dashboard", "Organiser");
                }

                // 3. If it failed, show us EXACTLY what the API said
                var errorDetails = await createResponse.Content.ReadAsStringAsync();
                ViewBag.Error = $"Failed to create event. API says: {errorDetails}";
            }

            return View(model);
        }
    }
}