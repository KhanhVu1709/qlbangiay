using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using QlBanGiay.Models;
using X.PagedList;
using System.Security.Cryptography;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace QlBanGiay.Areas.Admin.Controllers
{
    [Area("admin")]
    [Authorize(Roles = "quản lý sản phẩm")]
    public class ProductController : Controller
    {
        public QLBanGiayContext db = new QLBanGiayContext();

        public IActionResult DanhMucSanPham(int? page)
        {
            int pageSize = 10;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;
            var listSP = db.SanPhams.OrderBy(item => item.Id).ToList();
            PagedList<SanPham> list = new PagedList<SanPham>(listSP, pageNumber, pageSize);
            return View(list);
        }
        //
        public IActionResult create()
        {
            ViewBag.IdHang = new SelectList(db.Hangs.ToList(), "Id", "TenHang");
            
            ViewBag.action = "/admin/product/createpost";

            ViewBag.KichThuoc = db.KichThuocs.ToList();
            return View("ThemSanPham");
            
        }

        [HttpPost]
        public IActionResult createpost(IFormCollection fc)
        {
            //
            SanPham sp = new SanPham();
            string ten_sp = fc["TenSp"].ToString().Trim();
            int so_luong = Convert.ToInt32(fc["SoLuong"]);
            double gia_ban = Convert.ToDouble(fc["Gia"]);
            int thoi_luong = Convert.ToInt32(fc["ThoiGianBaoHanh"]);
            int id_hang = Convert.ToInt32(fc["IdHang"]);
            sp.TenSp = ten_sp;
            sp.SoLuong = so_luong;  
            sp.Gia = gia_ban;
            sp.ThoiGianBaoHanh = thoi_luong;
            sp.IdHang = id_hang;

            //
            List<string> fileNames = new List<string>();
            try
            {
                // Lặp qua tất cả các tệp tin được tải lên
                foreach (var file in Request.Form.Files)
                {
                    string fileName = file.FileName;
                    if (!string.IsNullOrEmpty(fileName))
                    {
                        var timestamp = DateTime.Now.ToFileTime();
                        fileName = timestamp + "_" + fileName;
                        string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images", fileName);
                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            file.CopyTo(stream);
                        }

                        // them tap tin vao danh sach
                        fileNames.Add(fileName);
                    }
                }

                if (fileNames.Count > 0)
                {
                    // them anh vao đối tượng sp
                    sp.Anh = fileNames[0];
                }
            }
            catch {; }
            db.SanPhams.Add(sp);
            db.SaveChanges();

            // lay id cua san pham vua them
            int id_sp = sp.Id;

            // them chi tiet anh
            for (var i = 1; i < fileNames.Count; i++)
            {
                Anh anh = new Anh();
                if (fileNames.Count > 0)
                {
                    anh.TenFileAnh = fileNames[i];
                }

                anh.IdSp = id_sp;
                db.Anhs.Add(anh);
                db.SaveChanges();
            }

            // size
            string[] kichThuocDaChon = fc["kichThuocs"];

            if(kichThuocDaChon != null && kichThuocDaChon.Length > 0)
            {
                foreach(var size in kichThuocDaChon)
                {
                    SanPhamKichThuoc sp_kt = new SanPhamKichThuoc();
                    sp_kt.IdSp = id_sp;
                    sp_kt.IdKichThuoc = Convert.ToInt32(size);

                    db.SanPhamKichThuocs.Add(sp_kt);
                    db.SaveChanges();
                }
            }

            return RedirectToAction("DanhMucSanPham", "Product");
        }

        public IActionResult Update(int id)
        {
            var sp = db.SanPhams.Find(id);

            //tìm size của sản phẩm theo id
            ViewBag.sizeDaChon = (from s in db.SanPhams
                                  join a in db.Anhs on s.Id equals a.IdSp into aGroup
                                  from a in aGroup.DefaultIfEmpty()
                                  join spkt in db.SanPhamKichThuocs on s.Id equals spkt.IdSp into spktGroup
                                  from spkt in spktGroup.DefaultIfEmpty()
                                  join kt in db.KichThuocs on spkt.IdKichThuoc equals kt.Id into ktGroup
                                  from kt in ktGroup.DefaultIfEmpty()
                                  where s.Id == id
                                  select kt.SoDo).ToList();

            ViewBag.IdHang = new SelectList(db.Hangs.ToList(), "Id", "TenHang");
            ViewBag.action = "/admin/product/updatepost";
            ViewBag.KichThuoc = db.KichThuocs.ToList();
            return View("ThemSanPham", sp);
        }

        [HttpPost]
        public IActionResult UpdatePost(int id, IFormCollection fc)
        {
            // tim san pham
            SanPham sp = db.SanPhams.Find(id);

            // 
            string ten_sp = fc["TenSp"].ToString().Trim();
            int so_luong = Convert.ToInt32(fc["SoLuong"]);
            double gia_ban = Convert.ToDouble(fc["Gia"]);
            int thoi_luong = Convert.ToInt32(fc["ThoiGianBaoHanh"]);
            int id_hang = Convert.ToInt32(fc["IdHang"]);
            sp.TenSp = ten_sp;
            sp.SoLuong = so_luong;
            sp.Gia = gia_ban;
            sp.ThoiGianBaoHanh = thoi_luong;
            sp.IdHang = id_hang;

            //
            List<string> fileNames = new List<string>();
            try
            {
                // Lặp qua tất cả các tệp tin được tải lên
                foreach (var file in Request.Form.Files)
                {
                    string fileName = file.FileName;
                    if (!string.IsNullOrEmpty(fileName))
                    {
                        var timestamp = DateTime.Now.ToFileTime();
                        fileName = timestamp + "_" + fileName;
                        string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images", fileName);
                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            file.CopyTo(stream);
                        }

                        // them tap tin vao danh sach
                        fileNames.Add(fileName);
                    }
                }

                if (fileNames.Count > 0)
                {
                    // them anh vao đối tượng sp
                    sp.Anh = fileNames[0];
                }
            }
            catch {; }
            db.SanPhams.Update(sp);
            db.SaveChanges();

            // remove size
            var sizeDaTonTai = db.SanPhamKichThuocs.Where(spkt => spkt.IdSp == id).ToList();
            foreach(var itemRemove in sizeDaTonTai)
            {
                db.SanPhamKichThuocs.Remove(itemRemove);
                db.SaveChanges();
            }

            // add size
            string[] kichThuocDaChon = fc["kichThuocs"];

            if (kichThuocDaChon != null && kichThuocDaChon.Length > 0)
            {
                foreach (var size in kichThuocDaChon)
                {
                    SanPhamKichThuoc sp_kt = new SanPhamKichThuoc();
                    sp_kt.IdSp = id;
                    sp_kt.IdKichThuoc = Convert.ToInt32(size);

                    db.SanPhamKichThuocs.Add(sp_kt);
                    db.SaveChanges();
                }
            }

            return RedirectToAction("DanhMucSanPham", "Product");
        }

        public IActionResult Delete(int id)
        {
            // xoa table anh theo id san pham
            var anhDaTonTai = db.Anhs.Where(a => a.IdSp == id).ToList();
            foreach(var itemAnh in anhDaTonTai)
            {
                db.Anhs.Remove(itemAnh); 
                db.SaveChanges();
            }

            // xoa table sanpham_kichthuoc theo id san pham
            var spkt = db.SanPhamKichThuocs.Where(spkt => spkt.IdSp == id).ToList();
            foreach(var itemPkt in spkt)
            {
                db.SanPhamKichThuocs.Remove(itemPkt); 
                db.SaveChanges();
            }

            SanPham sp = db.SanPhams.Find(id);
            db.SanPhams.Remove(sp);
            db.SaveChanges();
            return RedirectToAction("DanhMucSanPham", "Product");
        }
    }
}
