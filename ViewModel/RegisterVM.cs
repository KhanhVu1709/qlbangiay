using System.ComponentModel.DataAnnotations;

namespace QlBanGiay.ViewModel
{
    public class RegisterVM
    {
        [Required(ErrorMessage = "Tài khoản không được để trống")]
        public string taiKhoan { get; set; }
        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        public string matKhau { get; set; }
        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        [Compare("matKhau", ErrorMessage = "Mật khẩu và xác nhận mật khẩu không trùng khớp")]
        public string confirmMatKhau { get; set; }
        [Required(ErrorMessage = "Họ tên không được để trống")]
        public string hoTen { get; set; }
        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Không đúng định dạng email")]
        public string email { get; set; }
    }
}
