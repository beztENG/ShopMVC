using System.ComponentModel.DataAnnotations;

namespace ShopMVC.ViewModels
{
    public class LoginVM
    {
        [Display(Name = "Email ")]
        [Required(ErrorMessage = "Please enter username")]
        [MaxLength(20,ErrorMessage = "Maximun 20 characters")]
        public string Email { get; set; }

        [Display(Name = "Password")]
        [Required(ErrorMessage = "Please enter password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
