using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace QlBanGiay.Models
{
    public partial class User
    {
        public int Id { get; set; }

        [DisplayName("Tài khoản")]
        [Required(ErrorMessage = "Tài khoản không được để trống")]
        public string TaiKhoan { get; set; } = null!;

        [DisplayName("Mật khẩu")]
        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        public string MatKhau { get; set; } = null!;

        [DisplayName("Họ tên")]
        public string? HoTen { get; set; }

        [DisplayName("Email")]
        public string? Email { get; set; }

        [DisplayName("Địa chỉ")]
        public string? DiaChi { get; set; }

        [DisplayName("Số điện thoại")]
        public string? Sdt { get; set; }

		public string? Anh { get; set; }

		public bool? TrangThai { get; set; }

		public virtual ICollection<HoaDon> HoaDons { get; set; }
        public virtual ICollection<UserLoaiUser> UserLoaiUsers { get; set; }
    }
}
