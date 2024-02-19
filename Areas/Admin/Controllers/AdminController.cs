using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QlBanGiay.Models;

namespace QlBanGiay.Areas.Admin.Controllers
{
    [Area("admin")]
    [Route("admin")]
	[Authorize(Roles = "quản lý khách hàng, quản lý hoá đơn, quản lý sản phẩm", Policy = "AnyRole")]
	public class AdminController : Controller
    {
        QLBanGiayContext db = new QLBanGiayContext();
        [Route("index")]
        [Route("")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
