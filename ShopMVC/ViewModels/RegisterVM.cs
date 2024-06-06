using System;
using System.ComponentModel.DataAnnotations;

namespace ShopMVC.ViewModels
{
    public class RegisterViewModel
    {
        [Display(Name = "Username")]
        [Required(ErrorMessage = "Please enter a username.")]
        [MaxLength(20, ErrorMessage = "Maximum 20 characters allowed.")]
        public string UserName { get; set; } = null!;

        [Display(Name = "Password")]
        [Required(ErrorMessage = "Please enter a password.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;

        [Display(Name = "Full Name")] // More descriptive name
        [Required(ErrorMessage = "Please enter your full name.")]
        [MaxLength(50, ErrorMessage = "Maximum 50 characters allowed.")]
        public string FullName { get; set; } = null!;

        [Display(Name = "Gender")]
        public bool Gender { get; set; } // Made more explicit for true/false interpretation

        [Display(Name = "Birth Date")]
        [DataType(DataType.Date)]
        public DateTime? BirthDate { get; set; }

        [Display(Name = "Address")]
        [MaxLength(60, ErrorMessage = "Maximum 60 characters allowed.")]
        public string Address { get; set; } = null!; // Made optional for registration

        [Display(Name = "Phone Number")]
        [MaxLength(24, ErrorMessage = "Maximum 24 characters allowed.")]
        [Phone(ErrorMessage = "Invalid phone number format.")] // Simplified validation
        public string Phone { get; set; } // Made optional for registration

        [Display(Name = "Email")]
        [Required(ErrorMessage = "Please enter an email address.")]
        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        public string Email { get; set; } = null!;

        [Display(Name = "Profile Picture")]
        public string? Image { get; set; } // Optional during registration
    }
}
