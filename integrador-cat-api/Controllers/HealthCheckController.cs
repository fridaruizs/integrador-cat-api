using integrador_cat_api.Services;
using Microsoft.AspNetCore.Mvc;

namespace integrador_cat_api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HealthCheckController : ControllerBase
    {
        private readonly ICatService _catService;

        public HealthCheckController(ICatService catService)
        {
            _catService = catService;
        }


        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var appStatus = "Healthy";

            var isExternalApiHealthy = await _catService.CheckExternalApiHealthAsync();
            var externalApiStatus = isExternalApiHealthy ? "Healthy" : "Unreachable";

            var healthCheckResult = new
            {
                LocalAppStatus = appStatus,
                ExternalApiStatus = externalApiStatus
            };

            if (isExternalApiHealthy)
            {
                return Ok(healthCheckResult);
            }

            return StatusCode(503, healthCheckResult);
        }

    }
}
