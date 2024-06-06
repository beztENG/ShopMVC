﻿using System;
using System.Collections.Generic;

namespace ShopMVC.Data2;

public partial class VOrderDetail
{
    public int OrderDetailId { get; set; }

    public int OrderId { get; set; }

    public int ProductId { get; set; }

    public double UnitPrice { get; set; }

    public int Quantity { get; set; }

    public double Discount { get; set; }

    public string ProductName { get; set; } = null!;
}
