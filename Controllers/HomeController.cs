using Microsoft.AspNetCore.Mvc;

namespace Burg_Storage.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
