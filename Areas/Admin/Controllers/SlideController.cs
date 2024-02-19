using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QlBanGiay.Models;
using System.Data;
using X.PagedList;

namespace QlBanGiay.Areas.Admin.Controllers
{
	[Area("admin")]
	[Authorize(Roles = "quản lý khách hàng, quản lý hoá đơn, quản lý sản phẩm", Policy = "AnyRole")]
	public class SlideController : Controller
	{
		QLBanGiayContext db = new QLBanGiayContext();
		public IActionResult DanhMucSlide(int? page)
		{
			var pageSize = 10;
			var pageNumber = page == null || page < 0 ? 1 : page.Value;
			var listSlide = db.Slides.OrderBy(s => s.Id).ToList();
			PagedList<Slide> list = new PagedList<Slide>(listSlide, pageNumber, pageSize);
			return View(list);
		}

		public IActionResult create()
		{
			ViewBag.action = "/admin/slide/createpost";
			ViewBag.trangThaiSlide = db.TrangThaiSlides.ToList();
			return View("FormCreateUpdateSlide");
		}
		[HttpPost]
		public IActionResult createpost(IFormCollection fc)
		{
			Slide slide = new Slide();

			string title = Convert.ToString(fc["Title"]);
			int vi_tri = Convert.ToInt32(fc["ViTri"]);
			int id_trangThai = Convert.ToInt32(fc["TrangThai"]);

			string filename = "";

			try
			{
				filename = Request.Form.Files[0].FileName;
			} catch {; }

			if(!string.IsNullOrEmpty(filename))
			{
                // Tạo một timestamp để thêm vào tên tệp tin
                var timestamp = DateTime.Now.ToFileTime();
                filename = timestamp + "_" + filename;
                // Xác định đường dẫn lưu tệp tin
                string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images/Slide", filename);
                // Lưu tệp tin vào đường dẫn vừa xác định
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    Request.Form.Files[0].CopyTo(stream);
                }
                //update gia tri vao cot Anh trong csdl
                slide.Anh = filename;
            }

			slide.Title = title;
			slide.ViTri = vi_tri;
			slide.IdTrangThai = id_trangThai;

			db.Slides.Add(slide);
			db.SaveChanges();

			return RedirectToAction("DanhMucSlide", "Slide");
		}

		public IActionResult update(int id)
		{
			Slide slide = db.Slides.FirstOrDefault(s => s.Id == id);
			ViewBag.action = "/admin/slide/updatepost";
			ViewBag.trangThaiSlide = db.TrangThaiSlides.ToList();
			return View("FormCreateUpdateSlide", slide);
		}
		[HttpPost]
		public IActionResult updatepost(int id, IFormCollection fc)
		{
			Slide slide = db.Slides.FirstOrDefault(x => x.Id == id);

            string title = Convert.ToString(fc["Title"]);
            int vi_tri = Convert.ToInt32(fc["ViTri"]);
            int id_trangThai = Convert.ToInt32(fc["TrangThai"]);

            string filename = "";

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
                string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images/Slide", filename);
                // Lưu tệp tin vào đường dẫn vừa xác định
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    Request.Form.Files[0].CopyTo(stream);
                }
                //update gia tri vao cot Anh trong csdl
                slide.Anh = filename;
            }

            slide.Title = title;
            slide.ViTri = vi_tri;
            slide.IdTrangThai = id_trangThai;

            db.Slides.Update(slide);
            db.SaveChanges();

            return RedirectToAction("DanhMucSlide", "Slide");
        }

		public IActionResult delete(int id)
		{
			Slide slide = db.Slides.FirstOrDefault(s => s.Id == id);

			db.Slides.Remove(slide);
			db.SaveChanges();

			return RedirectToAction("DanhMucSlide", "Slide");
        }
	}
}
