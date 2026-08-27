using Microsoft.AspNetCore.Mvc;

namespace ParkingManagement.Web.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
