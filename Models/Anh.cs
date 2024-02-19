using System;
using System.Collections.Generic;

namespace QlBanGiay.Models
{
    public partial class Anh
    {
        public int Id { get; set; }
        public string? TenFileAnh { get; set; }
        public int? IdSp { get; set; }

        public virtual SanPham? IdSpNavigation { get; set; }
    }
}
