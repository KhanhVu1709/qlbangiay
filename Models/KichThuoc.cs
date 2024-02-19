using System;
using System.Collections.Generic;

namespace QlBanGiay.Models
{
    public partial class KichThuoc
    {
        public KichThuoc()
        {
            SanPhamKichThuocs = new HashSet<SanPhamKichThuoc>();
        }

        public int Id { get; set; }
        public int? SoDo { get; set; }

        public virtual ICollection<SanPhamKichThuoc> SanPhamKichThuocs { get; set; }
        public virtual ICollection<ChiTietHoaDon> ChiTietHoaDons { get; set; } = new List<ChiTietHoaDon>();
    }
}
