using Microsoft.AspNetCore.Mvc;

namespace integrador_cat_api.Controllers
{
    public class HealthCheckController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
