using Microsoft.AspNetCore.Mvc;

namespace RacedayMvc.Controllers
{
    [RoleAuthorize("Organiser")]
    public class OrganiserController : Controller
    {
        public IActionResult Dashboard() => View();
    }
}