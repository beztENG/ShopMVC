using System;
using System.Collections.Generic;

namespace ShopMVC.Data2;

public partial class Product
{
    public int ProductId { get; set; }

    public string ProductName { get; set; } = null!;

    public string? ProductAlias { get; set; }

    public int CategoryId { get; set; }

    public string? UnitDescription { get; set; }

    public double? UnitPrice { get; set; }

    public double ShippingFee { get; set; }

    public string? Image { get; set; }

    public DateTime ProductionDate { get; set; }

    public double Discount { get; set; }

    public int Views { get; set; }

    public string? Description { get; set; }

    public string SupplierId { get; set; } = null!;

    public virtual Category Category { get; set; } = null!;

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
}
