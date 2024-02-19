using System;
using System.Collections.Generic;

namespace QlBanGiay.Models
{
    public partial class SanPham
    {
        public SanPham()
        {
            Anhs = new HashSet<Anh>();
            ChiTietHoaDons = new HashSet<ChiTietHoaDon>();
            SanPhamKichThuocs = new HashSet<SanPhamKichThuoc>();
        }

        public int Id { get; set; }
        public string? TenSp { get; set; }
        public int? SoLuong { get; set; }
        public double? Gia { get; set; }
        public int? ThoiGianBaoHanh { get; set; }
        public string? Anh { get; set; }
        public int? IdHang { get; set; }

        public virtual Hang? IdHangNavigation { get; set; }
        public virtual ICollection<Anh> Anhs { get; set; }
        public virtual ICollection<ChiTietHoaDon> ChiTietHoaDons { get; set; }
        public virtual ICollection<SanPhamKichThuoc> SanPhamKichThuocs { get; set; }
    }
}
