using Blog.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Controllers;

[ApiController]
[Route("")]
public class HomeController : ControllerBase
{
    [HttpGet("health-check")]
    [ApiKey]
    public IActionResult HealthCheck()
    {
        return Ok("On");
    }
}