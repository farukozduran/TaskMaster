using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TaskMaster.MVC.Controllers
{
    [Authorize]
    public class ReportsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
