using Microsoft.AspNetCore.Mvc;

namespace Burg_Storage.Controllers
{
    public class FilesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
