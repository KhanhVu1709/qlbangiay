using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using QlBanGiay.Models;
using X.PagedList;

namespace QlBanGiay.Controllers
{
    public class ProductsController : Controller
    {
        public QLBanGiayContext db = new QLBanGiayContext();

        public IActionResult TrangChu(int? page)
        {
			var list = db.SanPhams.AsNoTracking().ToList();
			var pageSize = 4;
			var pageNumber = page == null || page < 0 ? 1 : page.Value;

			PagedList<SanPham> lsp = new PagedList<SanPham>(list, pageNumber, pageSize);
			return View(lsp);
        }

        public IActionResult ChiTietSP(int? id)
        {
            int _id = id ?? 0;
            // lấy duy nhất 1 bản ghi có id đã chọn
            SanPham item_sp = db.SanPhams.FirstOrDefault(i => i.Id == id);

			var kichThuoc = from sp in db.SanPhams
                            join spkt in db.SanPhamKichThuocs on sp.Id equals spkt.IdSp
                            join k in db.KichThuocs on spkt.IdKichThuoc equals k.Id
                            where sp.Id == _id
							select new
							{
								kichThuoc = k
							};
			// Lấy danh sách kích thước từ kết quả truy vấn LINQ
			var result = kichThuoc.ToList();

            // kiểm tra trong result chứa phần tử nào không => nếu có => true
			if (result.Any())
            {
                List<KichThuoc> kichThuocs = result.Select(r => r.kichThuoc).ToList();

                // Đưa thông tin vào viewdata
                ViewData["kichThuocs"] = kichThuocs;
            }

            // Lấy ảnh chi tiết sản phẩm
            var anhs = db.Anhs.Where(i => i.IdSp == _id).ToList();
            ViewData["anhs"] = anhs;
            return View(item_sp);
        }

        public IActionResult DanhMucSP(int? page, string searchString = "")
        {
			ViewBag.SearchString = searchString;

			// số lượng các phần tử của 1 trang 
			int pageSize = 8;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;

			if (!String.IsNullOrEmpty(searchString))
			{
				var pro = db.SanPhams.Where(x => x.TenSp.ToUpper().Contains(searchString.ToUpper()));
				PagedList<SanPham> find = new PagedList<SanPham>(pro, pageNumber, pageSize);
				return View(find);
			}

			var list = db.SanPhams.AsNoTracking().OrderBy(s => s.Id).ToList();
            PagedList<SanPham> lst = new PagedList<SanPham>(list, pageNumber, pageSize);
            return View(lst);
        }

        [HttpPost]
        public IActionResult GetFilterProducts([FromBody] FilterData filter)
        {
            var filterProducts = db.SanPhams.ToList();
            if(filter.priceRange != null && filter.priceRange.Count > 0 && !filter.priceRange.Contains("all"))
            {
                List<PriceRange> priceRanges = new List<PriceRange>();
                foreach(var range in filter.priceRange)
                {
                    var value = range.Split("-").ToArray();
                    PriceRange priceRange = new PriceRange();
                    priceRange.Min = Int16.Parse(value[0]);
                    priceRange.Max = Int16.Parse(value[1]);
                    priceRanges.Add(priceRange);
				}

                filterProducts = filterProducts.Where(p => priceRanges.Any(r => p.Gia >= r.Min && p.Gia <= r.Max)).ToList();
            }
				return PartialView("_ReturnProducts", filterProducts);
        }

	}
}
