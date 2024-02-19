using Microsoft.AspNetCore.Mvc;

namespace QlBanGiay.Areas.Admin.Controllers
{
    [Area("admin")]
    public class HomeController : Controller
    {
        public IActionResult TrangChu()
        {
            return View();
        }
    }
}
