using System.ComponentModel.DataAnnotations;

namespace ShopMVC.ViewModels
{
    public class CustomerProfileVM
    {
        [Display(Name = "CustomerID")]
        public string MaKh { get; set; }
        [Display(Name = "Password")]
        public string MatKhau { get; set; }
        [Display(Name = "Name")]
        public string HoTen { get; set; }
        [Display(Name = "Male ?")]
        public bool GioiTinh { get; set; }
        [Display(Name = "Birth")]
        public DateTime NgaySinh { get; set; }
        [Display(Name = "Address")]
        public string DiaChi { get; set; }
        [Display(Name = "Phone number")]
        public string DienThoai { get; set; }
        [Display(Name = "Email")]
        public string Email { get; set; }
        public string? Hinh { get; set; }
    }
}