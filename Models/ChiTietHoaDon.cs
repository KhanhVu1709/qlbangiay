using System;
using System.Collections.Generic;

namespace QlBanGiay.Models
{
    public partial class ChiTietHoaDon
    {
        public int Id { get; set; }
        public int? SlBan { get; set; }
        public double? DonGia { get; set; }
        public int? IdSp { get; set; }
        public int? IdHoaDon { get; set; }
        public int? IdKichThuoc { get; set; }
        public virtual KichThuoc? IdKichThuocNavigation { get; set; }
        public virtual HoaDon? IdHoaDonNavigation { get; set; }
        public virtual SanPham? IdSpNavigation { get; set; }
    }
}
