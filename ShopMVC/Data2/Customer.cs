using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ShopMVC.Data2;

public partial class Customer
{
    [Key] // Indicate it's the primary key
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string CustomerId { get; set; } = null!;

    public string? Password { get; set; }

    public string FullName { get; set; } = null!;

    public bool Gender { get; set; }

    public DateTime BirthDate { get; set; }

    public string? Address { get; set; }

    public string? Phone { get; set; }

    public string Email { get; set; } = null!;

    public string? Image { get; set; }

    public bool Active { get; set; }

    public int Role { get; set; }

    public string? RandomKey { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
