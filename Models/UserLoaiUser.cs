using System;
using System.Collections.Generic;

namespace QlBanGiay.Models
{
    public partial class UserLoaiUser
    {
        public int Id { get; set; }
        public int IdUser { get; set; }
        public int IdLoaiUser { get; set; }

        public virtual LoaiUser IdLoaiUserNavigation { get; set; } = null!;
        public virtual User IdUserNavigation { get; set; } = null!;
    }
}
