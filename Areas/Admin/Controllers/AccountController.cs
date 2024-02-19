﻿using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Mvc;
using QlBanGiay.Models;
using QlBanGiay.Areas.Admin.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace QlBanGiay.Areas.Admin.Controllers
{
	[Area("admin")]
	public class AccountController : Controller
	{
		QLBanGiayContext db = new QLBanGiayContext();
		public IActionResult Login()
		{
			ViewBag.action = "/admin/account/loginpost";
			return View();
		}

		[HttpPost]
		public IActionResult loginpost(IFormCollection fc)
		{
			string userName = fc["UserName"].ToString().Trim();
			string password = fc["Password"].ToString().Trim();

			var usersWithLoaiList = (from u in db.Users
                                     join ulu in db.UserLoaiUsers on u.Id equals ulu.IdUser into userLoaiGroups
                                     from ulu in userLoaiGroups.DefaultIfEmpty()
                                     join lu in db.LoaiUsers on ulu.IdLoaiUser equals lu.Id into loaiGroups
                                     from lu in loaiGroups.DefaultIfEmpty()
                                     where u.TaiKhoan == userName
                                     select new
                                     {
                                         Id = u.Id,
                                         TaiKhoan = u.TaiKhoan,
                                         MatKhau = u.MatKhau,
                                         TrangThai = u.TrangThai,
                                         Loai = lu.TenLoaiUser,
                                     }).ToList();

            if (usersWithLoaiList != null)
            {
                var userFirst = usersWithLoaiList.First(); // Lấy thông tin người dùng đầu tiên

                int id_user = userFirst.Id;
                foreach (var user in usersWithLoaiList)
                {
                    var passwordHasher = new PasswordHasher();

                    // Kiểm tra mật khẩu nhập vào có khớp với mật khẩu đã mã hoá hay không
                    var passwordVerification = passwordHasher.VerifyHashedPassword(user.MatKhau, password);

                    // Mật khẩu đúng => chuyển hướng đến trang khác
                    if (passwordVerification == Microsoft.AspNet.Identity.PasswordVerificationResult.Success)
                    {
                        if (user.TrangThai == false)
                        {
                            TempData["baned"] = "Tài khoản đã bị cấm";
                            return Redirect("/admin/account/login");
                        }
                        else
                        {
                            if(user.Loai == null)
                            {
								TempData["fail"] = "Bạn không có quyền truy cập admin";
								return Redirect("/admin/account/login");
							}

							var claim = new List<Claim>
							{
								new Claim(ClaimTypes.Name, user.TaiKhoan),
							};

							// Thêm các vai trò vào danh sách claim
							foreach (var userWithLoai in usersWithLoaiList)
							{
								if (userWithLoai.Loai != null)
								{
									claim.Add(new Claim(ClaimTypes.Role, userWithLoai.Loai.ToString()));
								}
							}

							var claimIdentity = new ClaimsIdentity(claim, CookieAuthenticationDefaults.AuthenticationScheme);
							HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimIdentity));

							// lưu session id và tên tài khoản
							HttpContext.Session.SetString("login", "true");
                            HttpContext.Session.SetInt32("id", id_user);
                            HttpContext.Session.SetString("taikhoan", user.TaiKhoan);
							if (user.Loai == "quản lý khách hàng")
							{
								return RedirectToAction("DanhMucTaiKhoan", "User");
							}
							if (user.Loai == "quản lý sản phẩm")
							{
								return RedirectToAction("DanhMucSanPham", "Product");
							}
							if (user.Loai == "quản lý hoá đơn")
							{
								return RedirectToAction("DanhMucGiaoDich", "Order");
							}

							return Redirect("/admin/");
						}
                    } else
                    {
                        TempData["fail"] = "Sai tài khoản hoặc mật khẩu";
                        return Redirect("/admin/account/login");
                    }
                }
            }

            return View();
		}

        public IActionResult Logout()
		{
			HttpContext.Session.Clear();
			HttpContext.SignOutAsync();
			return Redirect("/admin/account/login");
		}
	}
}
