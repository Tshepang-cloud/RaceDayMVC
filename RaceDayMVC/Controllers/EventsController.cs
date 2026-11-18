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

        [HttpPost]
        [RoleAuthorize("Organiser")]
        public async Task<IActionResult> Create(EventViewModel model, IFormFile bannerImage)
        {
            if (ModelState.IsValid)
            {
                // 1. Create the event
                var eventData = new { Name = model.Name, Date = model.Date, Location = model.Location };
                var createResponse = await _apiService.PostAsync("api/events", JsonContent.Create(eventData));

                if (createResponse.IsSuccessStatusCode)
                {
                    var createdEvent = await createResponse.Content.ReadFromJsonAsync<EventResponse>();

                    // 2. Handle Image Upload via API
                    if (bannerImage != null && bannerImage.Length > 0)
                    {
                        using var content = new MultipartFormDataContent();
                        using var stream = bannerImage.OpenReadStream();
                        var fileContent = new StreamContent(stream);
                        fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(bannerImage.ContentType);
                        content.Add(fileContent, "file", bannerImage.FileName);

                        var uploadResponse = await _apiService.PostAsync($"api/events/{createdEvent.Id}/upload-banner", content);

                        if (!uploadResponse.IsSuccessStatusCode)
                        {
                            ViewBag.Error = "Event created, but image upload failed.";
                            return View(model);
                        }
                    }
                    return RedirectToAction("Dashboard", "Organiser");
                }
            }
            return View(model);
        }
    }

    public class EventResponse
    {
        public int Id { get; set; }
    }
}