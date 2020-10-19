using Extractt.Web.Models;
using Extractt.Web.Services;
using Hangfire;
using Microsoft.AspNetCore.Mvc;

namespace Extractt.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("Ok");
        }
    }
}
