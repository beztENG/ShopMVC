using System;
using System.ComponentModel.DataAnnotations;

namespace ShopMVC.ViewModels
{
    public class CustomerProfileVM
    {
        [Display(Name = "Customer ID")]
        public string CustomerId { get; set; } = null!;

        [Display(Name = "Password")]
        [DataType(DataType.Password)] // For password masking in input fields
        public string Password { get; set; } = null!;

        [Display(Name = "Full Name")]
        public string FullName { get; set; } = null!;

        [Display(Name = "Gender")]
        public bool Gender { get; set; } // Consider using an enum for clarity

        [Display(Name = "Birth Date")]
        [DataType(DataType.Date)] // For date input fields
        public DateTime BirthDate { get; set; }

        [Display(Name = "Address")]
        public string Address { get; set; } = null!;

        [Display(Name = "Phone Number")]
        [Phone] // For phone number validation
        public string Phone { get; set; }

        [Display(Name = "Email")]
        [EmailAddress] // For email address validation
        public string Email { get; set; } = null!;

        [Display(Name = "Profile Image")]
        public string? Image { get; set; }
    }
}
