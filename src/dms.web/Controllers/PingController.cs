using Microsoft.AspNetCore.Mvc;

namespace dms.web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PingController : ControllerBase
    {
        private readonly ILogger<PingController> _logger;

        public PingController(ILogger<PingController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Get()
        {
            _logger.LogInformation($"Method {nameof(PingController.Get)}");
            return Ok("pong");
        }
    }
}
