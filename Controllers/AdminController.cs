using Microsoft.AspNetCore.Mvc;

namespace EMSystemTask.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
