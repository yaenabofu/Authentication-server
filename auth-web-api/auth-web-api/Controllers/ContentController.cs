using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace auth_web_api.Controllers
{
    [Authorize]
    public class ContentController : Controller
    {
        [HttpGet("content")]
        public IActionResult Index()
        {
            return Ok("CONTENT");
        }
    }
}
