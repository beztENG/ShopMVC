using System;
using System.Collections.Generic;

namespace ShopMVC.Data2;

public partial class Order
{
    public int OrderId { get; set; }

    public string CustomerId { get; set; } = null!;

    public DateTime OrderDate { get; set; }
 
    public DateTime? ShippedDate { get; set; }

    public string? FullName { get; set; }

    public string Address { get; set; } = null!;

    public string PaymentMethod { get; set; } = null!;

    public string ShippingMethod { get; set; } = null!;

    public double ShippingFee { get; set; }

    public string? Notes { get; set; }
    public bool Active { get; set; }

    public virtual Customer Customer { get; set; } = null!;

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
}
