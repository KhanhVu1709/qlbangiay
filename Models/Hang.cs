using System;
using System.Collections.Generic;

namespace QlBanGiay.Models
{
    public partial class Hang
    {
        public Hang()
        {
            SanPhams = new HashSet<SanPham>();
        }

        public int Id { get; set; }
        public string TenHang { get; set; } = null!;

        public virtual ICollection<SanPham> SanPhams { get; set; }
    }
}
