using Microsoft.AspNetCore.Mvc;
using QlBanGiay.Models;
using QlBanGiay.Areas.Admin.Models;
using X.PagedList;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Authorization;

namespace QlBanGiay.Areas.Admin.Controllers
{
    [Area("admin")]
    [Authorize(Roles = "quản lý hoá đơn")]
    public class OrderController : Controller
    {
        QLBanGiayContext db = new QLBanGiayContext();
        public IActionResult DanhMucGiaoDich(int? page)
        {
            int pageSize = 10;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;

            //ViewBag.trangThai = new SelectList(db.TrangThais.ToList(), "Id", "TenTrangThai");
            ViewBag.trangThai = db.TrangThais.ToList();

            var chiTietHoaDonSum = (from hd in db.HoaDons
                                    join cthd in db.ChiTietHoaDons on hd.Id equals cthd.IdHoaDon
                                    group cthd by cthd.IdHoaDon into g
                                    select new
                                    {
                                        IdHoaDon = g.Key,
                                        TongGiaTriHoaDon = g.Sum(ct => ct.DonGia * ct.SlBan)
                                    }).ToList();

            // chuyển chiTietHoaDonSum thành 1 đối tượng từ điển (dictionary) gồm key và value tương ứng
            var chiTietHoaDonSumDict = chiTietHoaDonSum.ToDictionary(item => item.IdHoaDon, item => item.TongGiaTriHoaDon);

            var linq = (from u in db.Users
                       join hd in db.HoaDons on u.Id equals hd.IdUser
                       join tt in db.TrangThais on hd.IdTrangThai equals tt.Id
                       select new ChiTietGiaoDich
                       {
                           IdHoaDon = hd.Id,
                           IdTaiKhoan = u.Id,
                           HoTen = u.HoTen,
                           Email = u.Email,
                           DiaChi = u.DiaChi,
                           Sdt = u.Sdt,
                           NgayBan = hd.NgayBan,
                           TrangThai = tt.TenTrangThai,
                           TongGiaTri = chiTietHoaDonSumDict.ContainsKey(hd.Id) ? chiTietHoaDonSumDict[hd.Id] : null,
                       }).ToList();
            PagedList<ChiTietGiaoDich> list = new PagedList<ChiTietGiaoDich>(linq, pageNumber, pageSize);
            return View(list);
        }

        [HttpPost]
        public IActionResult UpdateTrangThai(int trangThai, int hoaDonId)
        {
            try
            {
                HoaDon hoaDon = db.HoaDons.FirstOrDefault(hd => hd.Id == hoaDonId);
                if (hoaDon != null)
                {
                    hoaDon.IdTrangThai = trangThai;
                    db.HoaDons.Update(hoaDon);
                    db.SaveChanges();
                }
                return Json(new { success = true });
            } catch(Exception e)
            {
                return Json(new {success = false, message = e.Message});
            }
        }

        // xuất hoá đơn pdf
        public IActionResult Invoice(int idHoaDon)
        {
            // 
            ViewBag.chiTiet = (from ct in db.ChiTietHoaDons
                               join sp in db.SanPhams on ct.IdSp equals sp.Id
                               join kt in db.KichThuocs on ct.IdKichThuoc equals kt.Id
                               join hd in db.HoaDons on ct.IdHoaDon equals hd.Id
                               join tt in db.TrangThais on hd.IdTrangThai equals tt.Id
                               where idHoaDon == ct.IdHoaDon
                               select new
                               {
                                   TenSP = sp.TenSp,
                                   GiaSP = sp.Gia,
                                   SoLuong = ct.SlBan,
                                   KichThuocSP = kt.SoDo,
                                   TrangThaiSP = tt.TenTrangThai,
                               }).ToList();


            var chiTietHoaDonSum = (from hd in db.HoaDons
                                    join cthd in db.ChiTietHoaDons on hd.Id equals cthd.IdHoaDon
                                    group cthd by cthd.IdHoaDon into g
                                    select new
                                    {
                                        IdHoaDon = g.Key,
                                        TongGiaTriHoaDon = g.Sum(ct => ct.DonGia * ct.SlBan)
                                    }).ToList();

            // chuyển chiTietHoaDonSum thành 1 đối tượng từ điển (dictionary) gồm key và value tương ứng
            var chiTietHoaDonSumDict = chiTietHoaDonSum.ToDictionary(item => item.IdHoaDon, item => item.TongGiaTriHoaDon);

            // tim hoa don theo id
            var linq = (from u in db.Users
                        join hd in db.HoaDons on u.Id equals hd.IdUser
                        join tt in db.TrangThais on hd.IdTrangThai equals tt.Id
                        where hd.Id == idHoaDon
                        select new ChiTietGiaoDich
                        {
                            IdHoaDon = hd.Id,
                            IdTaiKhoan = u.Id,
                            HoTen = u.HoTen,
                            Email = u.Email,
                            DiaChi = u.DiaChi,
                            Sdt = u.Sdt,
                            NgayBan = hd.NgayBan,
                            TrangThai = tt.TenTrangThai,
                            TongGiaTri = chiTietHoaDonSumDict.ContainsKey(hd.Id) ? chiTietHoaDonSumDict[hd.Id] : null,
                        }).FirstOrDefault();

            return View(linq);
        }
    }
}
