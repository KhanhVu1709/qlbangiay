using Azure;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QlBanGiay.Models;
using System.Data;
using System.Runtime.Intrinsics.Arm;
using X.PagedList;

namespace QlBanGiay.Areas.Admin.Controllers
{
    [Area("admin")]
    [Authorize(Roles = "quản lý khách hàng")]
    public class UserController : Controller
    {
        QLBanGiayContext db = new QLBanGiayContext();
        public IActionResult DanhMucTaiKhoan(int? page)
        {
            int pageSize = 10;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;
            var listUser = db.Users.OrderBy(item => item.Id).ToList();
            PagedList<User> list = new PagedList<User>(listUser, pageNumber, pageSize);
            return View(list);
        }

        public IActionResult create()
        {
            ViewBag.action = "/admin/user/createpost";
            return View("FormCreateUpdate");
        }

        [HttpPost]
        public IActionResult createpost(IFormCollection fc)
        {
            // tim doi tuong theo id
            User user = new User();

            // 
            string tai_khoan = fc["TaiKhoan"].ToString().Trim();
            string mat_Khau = fc["MatKhau"].ToString();
            string ho_ten = fc["HoTen"].ToString().Trim();
            string email = fc["Email"].ToString().Trim();
            string dia_chi = fc["DiaChi"].ToString().Trim();
            string sdt = fc["Sdt"].ToString().Trim();
            bool trang_thai = Convert.ToBoolean(Convert.ToInt32(fc["TrangThai"]));

            string filename = "";
            
            // kiểm tra và lấy tệp tin
            try
            {
                filename = Request.Form.Files[0].FileName;
            }
            catch {; }
            if (!string.IsNullOrEmpty(filename))
            {
                // Tạo một timestamp để thêm vào tên tệp tin
                var timestamp = DateTime.Now.ToFileTime();
                filename = timestamp + "_" + filename;
                // Xác định đường dẫn lưu tệp tin
                string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images/User", filename);
                // Lưu tệp tin vào đường dẫn vừa xác định
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    Request.Form.Files[0].CopyTo(stream);
                }
                //update gia tri vao cot Anh trong csdl
                user.Anh = filename;
            }

            // 
            // kiểm tra tài khoản đã tồn tại chưa
            var existingUser = db.Users.FirstOrDefault(u => u.TaiKhoan == tai_khoan);
            if (existingUser != null)
            {
                TempData["Error"] = "Đã tồn tại tài khoản này rồi.";
                return RedirectToAction("Create", "User");
            }
            else
            {
                string plainPassword = mat_Khau;
                // Tạo một instance của PasswordHasher
                var passwordHasher = new PasswordHasher();
                // Mã hoá mật khẩu
                string hashedPassword = passwordHasher.HashPassword(plainPassword);
                user.TaiKhoan = tai_khoan;
                user.MatKhau = hashedPassword;
                user.HoTen = ho_ten;
                user.Email = email;
                user.Sdt = sdt;
                user.TrangThai = trang_thai;
                db.Add(user);
                db.SaveChanges();
                return RedirectToAction("DanhMucTaiKhoan", "User");
            }
        }

        public IActionResult update(int id)
        {
            // tim doi tuong theo id
            User user = db.Users.FirstOrDefault(u => u.Id == id);
            ViewBag.action = "/admin/user/updatepost";
            return View("FormCreateUpdate", user);
        }

        [HttpPost]
        public IActionResult updatepost(int id, IFormCollection fc)
        {
            // tim doi tuong theo id
            User user = db.Users.FirstOrDefault(u => u.Id == id);

            string tai_khoan = fc["TaiKhoan"].ToString();
            string mat_Khau = fc["MatKhau"].ToString();
            string ho_ten = fc["HoTen"].ToString().Trim();
            string email = fc["Email"].ToString().Trim();
            string dia_chi = fc["DiaChi"].ToString().Trim();
            string sdt = fc["Sdt"].ToString().Trim();
            bool trang_thai = Convert.ToBoolean(Convert.ToInt32(fc["TrangThai"]));

            if(!String.IsNullOrEmpty(mat_Khau))
            {
                //
                string plainPassword = mat_Khau;
                // Tạo một instance của PasswordHasher
                var passwordHasher = new PasswordHasher();
                // Mã hoá mật khẩu
                string hashedPassword = passwordHasher.HashPassword(plainPassword);
                user.MatKhau = hashedPassword;
            }

            string filename = "";
			// kiểm tra và lấy tệp tin
			try
			{
				filename = Request.Form.Files[0].FileName;
			}
			catch {; }
			if (!string.IsNullOrEmpty(filename))
			{
				// Tạo một timestamp để thêm vào tên tệp tin
				var timestamp = DateTime.Now.ToFileTime();
				filename = timestamp + "_" + filename;
				// Xác định đường dẫn lưu tệp tin
				string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images/User", filename);
				// Lưu tệp tin vào đường dẫn vừa xác định
				using (var stream = new FileStream(path, FileMode.Create))
				{
					Request.Form.Files[0].CopyTo(stream);
				}
				//update gia tri vao cot Anh trong csdl
				user.Anh = filename;
			}

			user.TaiKhoan = tai_khoan;
            user.HoTen = ho_ten;
            user.Email = email;
            user.DiaChi = dia_chi;
            user.Sdt = sdt;
            user.TrangThai = trang_thai;

            db.Users.Update(user);
            db.SaveChanges();

            return RedirectToAction("DanhMucTaiKhoan", "User");
        }

        public IActionResult delete(int id)
        {
            User user = db.Users.FirstOrDefault(u => u.Id == id);
            db.Remove(user);
            db.SaveChanges();
            return RedirectToAction("DanhMucTaiKhoan", "User");
        }

        // phân quyền tài khoản
        public IActionResult PhanQuyenTaiKhoan(int id)
        {
            User user = db.Users.FirstOrDefault(u => u.Id == id);

            ViewBag.QuyenDaChon = (from u in db.Users
                                   join ulu in db.UserLoaiUsers on u.Id equals ulu.IdUser into uluGroup
                                   from ulu in uluGroup.DefaultIfEmpty()
                                   join lu in db.LoaiUsers on ulu.IdLoaiUser equals lu.Id into luGroup
                                   from lu in luGroup.DefaultIfEmpty()
                                   where u.Id == id
                                   select lu.TenLoaiUser).ToList();

            ViewBag.action = "/admin/user/phanquyenpost";

            return View(user);
        }

        public IActionResult phanquyenpost(int id, IFormCollection fc)
        {
            User user = db.Users.FirstOrDefault(u => u.Id == id);
            //remove quyen
            var quyenDaTonTai = db.UserLoaiUsers.Where(q => q.IdUser == id).ToList();
            foreach(var quyenRemove in quyenDaTonTai)
            {
                db.UserLoaiUsers.Remove(quyenRemove);
                db.SaveChanges();
            }

            // lay ra nhung input name=loaiuser da duoc checked
            // add quyen
            string[] quyen = fc["loaiuser"];
            if(quyen != null && quyen.Length > 0)
            {
                foreach(var item in quyen)
                {
                    UserLoaiUser u = new UserLoaiUser();

                    u.IdUser = id;
                    u.IdLoaiUser = Convert.ToInt32(item);

                    db.UserLoaiUsers.Add(u); 
                    db.SaveChanges();
                }
            }

            return RedirectToAction("DanhMucTaiKhoan", "User");
        }
    }
}
