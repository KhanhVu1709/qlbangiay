using JetBrains.Annotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using QlBanGiay.Models;
using QlBanGiay.Service;
using System.Globalization;

namespace QlBanGiay.Controllers
{
	public class CartController : Controller
	{
		QLBanGiayContext db = new QLBanGiayContext();
        private readonly IVnPayService _vnPayService;

        public CartController(IVnPayService vnPayService)
		{
			_vnPayService = vnPayService;

        }

		// cart
		#region
        public IActionResult Index()
		{
			List<Item> cart = Cart.GetCart(HttpContext.Session);
			if (cart != null)
			{
				ViewBag.Cart = cart;
			}
			return View();
		}

		// checkout
		[Authorize]
        public IActionResult Checkout()
		{
			List<Item> cart = Cart.GetCart(HttpContext.Session);
			if (cart != null)
			{
				ViewBag.Cart = cart;
			}
			return View();
		}
		[HttpPost]
		public IActionResult Checkout(IFormCollection fc)
		{
			int id_user = Convert.ToInt32(fc["id"]);
			DateTime ngayDat = Convert.ToDateTime(fc["ngayDat"]);
			string payment = Convert.ToString(fc["payment"]);

			HttpContext.Session.SetInt32("IdUser", id_user);
			HttpContext.Session.SetString("NgayDat", ngayDat.ToString());

			// thánh toán VNPAY
			if (payment == "Thanh toán VNPay")
            {
				User user = db.Users.FirstOrDefault(u => u.Id == id_user);
				var vnPayModel = new PaymentInfomationModel
				{
					Amount = Cart.CartTotal(HttpContext.Session),
					CreatedDate = DateTime.Now,
					OrderDescription = $"{user.HoTen} {user.Sdt} {user.DiaChi}",
					Name = user.HoTen,
					OrderId = new Random().Next(1000, 10000)
				};
				return Redirect(_vnPayService.CreatePaymentUrl(HttpContext, vnPayModel));
            }

            HoaDon hd = new HoaDon();
			hd.IdUser = id_user;
			hd.NgayBan = ngayDat;
			hd.IdTrangThai = 1;
			db.HoaDons.Add(hd);
			db.SaveChanges();

			// lấy ra hoá đơn mới
			int id_hdNew = hd.Id;

			List<Item> cart = Cart.GetCart(HttpContext.Session);

			if (cart != null)
			{
				foreach (var line in cart)
				{
					var so_luong = line.soLuong;
					var don_gia = line.sanPham.Gia;
					var id_sp = line.sanPham.Id;
					var id_hd = id_hdNew;
					var id_kt = line.kichThuoc.Id;

					ChiTietHoaDon ct = new ChiTietHoaDon();
					ct.SlBan = so_luong;
					ct.DonGia = don_gia;
					ct.IdSp = id_sp;
					ct.IdHoaDon = id_hd;
					ct.IdKichThuoc = id_kt;

					db.ChiTietHoaDons.Add(ct);
					db.SaveChanges();
				}

				Cart.CartDestroy(HttpContext.Session);
				return Redirect("/Cart/OrderComplete");
			}

			return View();
		}

		// OrderComplete
		[Authorize]
		public IActionResult OrderComplete()
		{
			return View();
		}

		public IActionResult AddToCart(int id, int idSize, int quantity)
		{
			Cart.AddItem(HttpContext.Session, id, idSize, quantity);
			return RedirectToAction("Index");
		}

		public IActionResult UnAddToCart(int id, int idSize)
		{
			Cart.UnAddItem(HttpContext.Session, id, idSize);
			return RedirectToAction("Index");
		}

		public IActionResult UpdateCart()
		{
			List<Item> cart = Cart.GetCart(HttpContext.Session);
			foreach (var item in cart)
			{
				int soLuong = Convert.ToInt32(Request.Form["product_" + item.sanPham.Id]);
				Cart.CartUpdate(HttpContext.Session, item.sanPham.Id, soLuong);
			}
			return RedirectToAction("Index");
		}

		public IActionResult RemoveFromCart(int id, int idSize)
		{
			Cart.RemoveLine(HttpContext.Session, id, idSize);
			return RedirectToAction("Index");
		}

		public IActionResult Destroy()
		{
			Cart.CartDestroy(HttpContext.Session);
			return RedirectToAction("Index");
		}
		#endregion

		// payment
		[Authorize]
        public IActionResult PaymentCallBack()
		{
			var response = _vnPayService.PaymentExecute(Request.Query);

			if(response == null || response.VnPayResponseCode != "00")
			{
				TempData["Message"] = $"Lỗi thanh toán VNPay {response.VnPayResponseCode}";
				return RedirectToAction("PaymentFail");
			}

			// lưu đơn hàng thành công
			int id_user = (int)HttpContext.Session.GetInt32("IdUser");
			DateTime ngayDat = Convert.ToDateTime(HttpContext.Session.GetString("NgayDat"));

			HoaDon hd = new HoaDon();
			hd.IdUser = id_user;
			hd.NgayBan = ngayDat;
			hd.IdTrangThai = 2;
			db.HoaDons.Add(hd);
			db.SaveChanges();

			// lấy ra hoá đơn mới
			int id_hdNew = hd.Id;

			List<Item> cart = Cart.GetCart(HttpContext.Session);

			if (cart != null)
			{
				foreach (var line in cart)
				{
					var so_luong = line.soLuong;
					var don_gia = line.sanPham.Gia;
					var id_sp = line.sanPham.Id;
					var id_hd = id_hdNew;
					var id_kt = line.kichThuoc.Id;

					ChiTietHoaDon ct = new ChiTietHoaDon();
					ct.SlBan = so_luong;
					ct.DonGia = don_gia;
					ct.IdSp = id_sp;
					ct.IdHoaDon = id_hd;
					ct.IdKichThuoc = id_kt;

					db.ChiTietHoaDons.Add(ct);
					db.SaveChanges();
				}

				Cart.CartDestroy(HttpContext.Session);
				return Redirect("/Cart/OrderComplete");
			}

			return View();
		}

		[Authorize]
		public IActionResult PaymentFail()
		{
			return View();
		}
    }
}
