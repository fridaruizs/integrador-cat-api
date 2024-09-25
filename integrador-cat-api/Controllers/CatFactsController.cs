using Microsoft.AspNetCore.Mvc;

namespace integrador_cat_api.Controllers
{
    public class CatFactsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
