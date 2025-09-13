using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Burg_Storage.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public IActionResult Index() => RedirectToAction("Index", "Documents");
    }
}
