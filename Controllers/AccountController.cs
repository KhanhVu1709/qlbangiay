using Microsoft.AspNetCore.Mvc;
using QlBanGiay.ViewModel;
using Microsoft.AspNet.Identity;
using QlBanGiay.Models;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;

namespace QlBanGiay.Controllers
{
    public class AccountController : Controller
    {
        public readonly IHttpContextAccessor contxt;

        public AccountController(IHttpContextAccessor httpContextAccessor)
        {
            contxt = httpContextAccessor;
        }

        QLBanGiayContext db = new QLBanGiayContext();

        // register
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(RegisterVM rvm) 
        {
            // kiểm tra tài khoản đã tồn tại chưa
            var existingUser = db.Users.FirstOrDefault(u => u.TaiKhoan == rvm.taiKhoan);
            if (existingUser != null)
            {
                TempData["Error"] = "Đã tồn tại tài khoản này rồi.";
                return View();
            } else
            {
                string plainPassword = rvm.matKhau;
                // Tạo một instance của PasswordHasher
                var passwordHasher = new PasswordHasher();
                // Mã hoá mật khẩu
                string hashedPassword = passwordHasher.HashPassword(plainPassword);
                if (ModelState.IsValid)  // thông tin vượt qua được validation
                {
                    User user = new User()
                    {
                        TaiKhoan = rvm.taiKhoan,
                        MatKhau = hashedPassword,
                        HoTen = rvm.hoTen,
                        Email = rvm.email,
                    };
                    db.Add(user);
                    db.SaveChanges();
                    return RedirectToAction("Login");
                }
                else
                {
                    ModelState.AddModelError("New error", "Invalid data");
                    return View();
                }
            }
        }

        // login
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(User user)
        {
            if(user.TaiKhoan == null)
            {
                TempData["TK"] = "Tài khoản không được để trống";
            }
            else if(user.MatKhau == null)
            {
                TempData["MK"] = "Mật khẩu không được để trống";
            }
            else
            {
                var item = db.Users.FirstOrDefault(u => u.TaiKhoan == user.TaiKhoan);

				var usersWithLoaiList = (from u in db.Users
										 join ulu in db.UserLoaiUsers on u.Id equals ulu.IdUser into userLoaiGroups
										 from ulu in userLoaiGroups.DefaultIfEmpty()
										 join lu in db.LoaiUsers on ulu.IdLoaiUser equals lu.Id into loaiGroups
										 from lu in loaiGroups.DefaultIfEmpty()
										 where u.TaiKhoan == user.TaiKhoan
										 select new
										 {
											 Id = u.Id,
											 TaiKhoan = u.TaiKhoan,
											 MatKhau = u.MatKhau,
											 TrangThai = u.TrangThai,
											 Loai = lu.TenLoaiUser,
										 }).ToList();

				if (item != null)
                {
                    int id_user = item.Id;
                    var passwordHasher = new PasswordHasher();

                    // Kiểm tra mật khẩu nhập vào có khớp với mật khẩu đã mã hoá hay không
                    var passwordVerification = passwordHasher.VerifyHashedPassword(item.MatKhau, user.MatKhau);

                    // Mật khẩu đúng => chuyển hướng đến trang khác
                    if (passwordVerification == Microsoft.AspNet.Identity.PasswordVerificationResult.Success)
                    {
                        if (item.TrangThai == false)
                        {
                            TempData["baned"] = "Tài khoản đã bị cấm";
                            return View();
                        } else
                        {

                            var claim = new List<Claim>
                            {
                                new Claim(ClaimTypes.Name, item.TaiKhoan),
                            };

                            // Thêm các vai trò vào danh sách claim
                            foreach(var userWithLoai in usersWithLoaiList)
                            {
                                if(userWithLoai.Loai != null)
                                {
									claim.Add(new Claim(ClaimTypes.Role, userWithLoai.Loai.ToString()));
								}
                            }

                            var claimIdentity = new ClaimsIdentity(claim, CookieAuthenticationDefaults.AuthenticationScheme);
                            HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimIdentity));

                            contxt.HttpContext.Session.SetString("login", "true");
                            contxt.HttpContext.Session.SetString("taikhoan", item.TaiKhoan);
                            contxt.HttpContext.Session.SetInt32("idUser", id_user);

                            return RedirectToAction("TrangChu", "Products");
                        }
                    }
                }

                // Mật khẩu không khớp hoặc không tìm thấy người dùng
                TempData["SaiMK"] = "Sai tài khoản hoặc mật khẩu";
            }
            // Trả về view Login để nhập lại thông tin đăng nhập
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            HttpContext.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }
    }
}
