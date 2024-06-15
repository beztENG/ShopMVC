using System.ComponentModel.DataAnnotations;

namespace ShopMVC.ViewModels
{
    public class LoginVM
    {
        [Display(Name = "Email ")]
        [Required(ErrorMessage = "Please enter username")]
        [MaxLength(50,ErrorMessage = "Maximun 50 characters")]
        public string Email { get; set; }

        [Display(Name = "Password")]
        [Required(ErrorMessage = "Please enter password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
