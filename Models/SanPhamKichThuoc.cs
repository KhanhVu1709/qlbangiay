using System;
using System.Collections.Generic;

namespace QlBanGiay.Models
{
    public partial class SanPhamKichThuoc
    {
        public int Id { get; set; }
        public int IdSp { get; set; }
        public int IdKichThuoc { get; set; }
		public int? SoLuong { get; set; }
		public virtual KichThuoc IdKichThuocNavigation { get; set; } = null!;
        public virtual SanPham IdSpNavigation { get; set; } = null!;
    }
}
