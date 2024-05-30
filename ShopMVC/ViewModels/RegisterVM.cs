using System;
using System.ComponentModel.DataAnnotations;

namespace ShopMVC.ViewModels
{
    public class RegisterVM
    {
        [Display(Name = "Username")]
        [Required(ErrorMessage = "Please enter username")]
        [MaxLength(20, ErrorMessage = "Maximum 20 characters")]
        public string MaKh { get; set; }

        [Display(Name = "Password")]
        [Required(ErrorMessage = "Please enter password")]
        [DataType(DataType.Password)]
        public string MatKhau { get; set; }

        [Display(Name = "Your name")]
        [Required(ErrorMessage = "Please enter your name")]
        [MaxLength(50, ErrorMessage = "Maximum 50 characters")]
        public string HoTen { get; set; }

        [Display(Name = "Gender")]
        public bool GioiTinh { get; set; } = true;

        [Display(Name = "Date of Birth")]
        [DataType(DataType.Date)]
        public DateTime? NgaySinh { get; set; }

        [MaxLength(60, ErrorMessage = "Maximum 60 characters")]
        [Display(Name = "Address")]
        public string DiaChi { get; set; }

        [Display(Name = "Phone number")]
        [MaxLength(24, ErrorMessage = "Maximum 24 characters")]
        [RegularExpression(@"0[9875]\d{8}", ErrorMessage = "Phone number format is not correct")]
        public string DienThoai { get; set; }

        [EmailAddress(ErrorMessage = "Your Email format is not correct")]
        public string Email { get; set; }

        [Display(Name = "Picture")]
        public string? Hinh { get; set; }
    }
}
