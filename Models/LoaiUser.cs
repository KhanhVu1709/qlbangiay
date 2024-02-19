using System;
using System.Collections.Generic;

namespace QlBanGiay.Models
{
    public partial class LoaiUser
    {
        public LoaiUser()
        {
            UserLoaiUsers = new HashSet<UserLoaiUser>();
        }

        public int Id { get; set; }
        public string TenLoaiUser { get; set; } = null!;

        public virtual ICollection<UserLoaiUser> UserLoaiUsers { get; set; }
    }
}
