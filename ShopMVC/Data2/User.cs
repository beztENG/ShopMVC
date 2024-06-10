using System.ComponentModel.DataAnnotations;

namespace ShopMVC.Data2
{
    public partial class User
    {
        public int UserId { get; set; }

        [Required]
        [StringLength(20)]
        public string CustomerId { get; set; }

        [Required]
        [StringLength(100)]
        public string Password { get; set; }

        [Required]
        [StringLength(100)]
        [EmailAddress]
        public string Email { get; set; }

        public int Role { get; set; }
        public bool Active { get; set; }

        public virtual Customer Customer { get; set; }  // Navigation property for the relationship
    }

}
