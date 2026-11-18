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
                var userId = HttpContext.Session.GetInt32("UserId");
                var eventData = new { EventName = model.Name, Date = model.Date, Location = model.Location, OrganiserId = userId };
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

                        await _apiService.PostAsync($"api/events/{createdEvent.Id}/upload-banner", content);
                    }
                    return RedirectToAction("Dashboard", "Organiser");
                }
                ViewBag.Error = await createResponse.Content.ReadAsStringAsync();
            }
            return View(model);
        }
    }
}