using System;
using System.Collections.Generic;

namespace QlBanGiay.Models;

public partial class TrangThai
{
    public int Id { get; set; }

    public string? TenTrangThai { get; set; }

    public virtual ICollection<HoaDon> HoaDons { get; set; } = new List<HoaDon>();
}
