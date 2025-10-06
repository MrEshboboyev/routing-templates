using Microsoft.AspNetCore.Mvc;

namespace RoutingExperience.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class EnvironmentController(
    IWebHostEnvironment env,
    ILogger<EnvironmentController> logger
) : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        // Log the environment name
        logger.LogInformation("Environment: {EnvironmentName}", env.EnvironmentName);

        // Return environment details
        var environmentDetails = new
        {
            env.EnvironmentName,
            env.ApplicationName,
            env.ContentRootPath,
            env.WebRootPath
        };

        return Ok(environmentDetails);
    }
}
