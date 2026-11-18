using Microsoft.AspNetCore.Mvc;

namespace RacedayMvc.Controllers
{
    [RoleAuthorize("Participant")]
    public class ParticipantController : Controller
    {
        public IActionResult Index() => View();
        public IActionResult MyEnrollments() => View();
    }
}