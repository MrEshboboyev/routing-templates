using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace RoutingExperience.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnvironmentController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<EnvironmentController> _logger;

        public EnvironmentController(IWebHostEnvironment env, ILogger<EnvironmentController> logger)
        {
            _env = env;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Get()
        {
            // Log the environment name
            _logger.LogInformation("Environment: {EnvironmentName}", _env.EnvironmentName);

            // Return environment details
            var environmentDetails = new
            {
                EnvironmentName = _env.EnvironmentName,
                ApplicationName = _env.ApplicationName,
                ContentRootPath = _env.ContentRootPath,
                WebRootPath = _env.WebRootPath
            };

            return Ok(environmentDetails);
        }
    }
}
