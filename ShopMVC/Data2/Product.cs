using System.ComponentModel.DataAnnotations;
using ShopMVC.Data2;

namespace ShopMVC.Data2;

public partial class Product
{
    public int ProductId { get; set; }

    [Required]
    public string ProductName { get; set; } = null!;

    public string? ProductAlias { get; set; }

    [Required]
    public int CategoryId { get; set; }

    public string? UnitDescription { get; set; }

    public double? UnitPrice { get; set; }

    public double ShippingFee { get; set; }

    public string? Image { get; set; }

    public DateTime ProductionDate { get; set; }

    public double Discount { get; set; }

    public int Views { get; set; }

    public string? Description { get; set; }
    public double? Profit { get; set; }
    public int? QuantitySold { get; set; }

    [Required]
    public string SupplierId { get; set; } = null!;
    public bool Active { get; set; }


    public virtual Category Category { get; set; } = null!;

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
}
