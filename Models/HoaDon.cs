using System;
using System.Collections.Generic;
using QlBanGiay.Models;

namespace QlBanGiay.Models
{
    public partial class HoaDon
    {
        public int Id { get; set; }
        public DateTime? NgayBan { get; set; }
        public int? IdUser { get; set; }
        public int? IdTrangThai { get; set; }
        public virtual ICollection<ChiTietHoaDon> ChiTietHoaDons { get; set; } = new List<ChiTietHoaDon>();
        public virtual TrangThai? IdTrangThaiNavigation { get; set; }
        public virtual User? IdUserNavigation { get; set; }
    }
}
