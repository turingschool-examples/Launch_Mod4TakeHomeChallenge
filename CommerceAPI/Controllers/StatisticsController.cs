using Microsoft.AspNetCore.Mvc;

namespace CommerceAPI.Controllers
{
    public class StatisticsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
