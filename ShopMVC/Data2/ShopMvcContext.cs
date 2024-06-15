using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ShopMVC.Data2;

public partial class ShopMvcContext : DbContext
{
    public ShopMvcContext()
    {
    }

    public ShopMvcContext(DbContextOptions<ShopMvcContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderDetail> OrderDetails { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<VOrderDetail> VOrderDetails { get; set; }
    public virtual DbSet<User> Users { get; set; }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=.;Initial Catalog=ShopMVC;Integrated Security=True;Trust Server Certificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK_Categories");

            entity.ToTable("Category");

            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.CategoryName).HasMaxLength(50);
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.CustomerId).HasName("PK_Customers");

            entity.ToTable("Customer");

            entity.Property(e => e.CustomerId)
                .HasMaxLength(20)
                .HasColumnName("CustomerID");
            entity.Property(e => e.Address).HasMaxLength(60);
            entity.Property(e => e.BirthDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(50);
            entity.Property(e => e.FullName).HasMaxLength(50);
            entity.Property(e => e.Image)
                .HasMaxLength(50)
                .HasDefaultValue("Photo.gif");
            entity.Property(e => e.Password).HasMaxLength(50);
            entity.Property(e => e.Phone).HasMaxLength(24);
            entity.Property(e => e.RandomKey)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("PK_Orders");

            entity.ToTable("Order");

            entity.Property(e => e.OrderId).HasColumnName("OrderID");
            entity.Property(e => e.Address).HasMaxLength(60);
            entity.Property(e => e.CustomerId)
                .HasMaxLength(20)
                .HasColumnName("CustomerID");
            entity.Property(e => e.FullName).HasMaxLength(50);
            entity.Property(e => e.Notes).HasMaxLength(50);
            entity.Property(e => e.OrderDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.PaymentMethod)
                .HasMaxLength(50)
                .HasDefaultValue("Cash");
            entity.Property(e => e.ShippedDate)
                .HasDefaultValueSql("(((1)/(1))/(1900))")
                .HasColumnType("datetime");
            entity.Property(e => e.ShippingMethod)
                .HasMaxLength(50);

            entity.HasOne(d => d.Customer).WithMany(p => p.Orders)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("FK_Order_Customer");
        });

        modelBuilder.Entity<OrderDetail>(entity =>
        {
            entity.HasKey(e => e.OrderDetailId).HasName("PK_OrderDetails");

            entity.ToTable("OrderDetail");

            entity.Property(e => e.OrderDetailId).HasColumnName("OrderDetailID");
            entity.Property(e => e.OrderId).HasColumnName("OrderID");
            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.Quantity).HasDefaultValue(1);

            entity.HasOne(d => d.Order).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("FK_OrderDetail_Order");

            entity.HasOne(d => d.Product).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK_OrderDetail_Product");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProductId).HasName("PK_Products");

            entity.ToTable("Product");

            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.Image).HasMaxLength(50);
            entity.Property(e => e.ProductAlias).HasMaxLength(50);
            entity.Property(e => e.ProductName).HasMaxLength(50);
            entity.Property(e => e.ProductionDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.SupplierId)
                .HasMaxLength(50)
                .HasColumnName("SupplierID");
            entity.Property(e => e.UnitDescription).HasMaxLength(50);
            entity.Property(e => e.UnitPrice).HasDefaultValue(0.0);

            entity.HasOne(d => d.Category).WithMany(p => p.Products)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("FK_Product_Category");
        });

        modelBuilder.Entity<VOrderDetail>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vOrderDetails");

            entity.Property(e => e.OrderDetailId).HasColumnName("OrderDetailID");
            entity.Property(e => e.OrderId).HasColumnName("OrderID");
            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.ProductName).HasMaxLength(50);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK_Users"); // Primary key is UserId (int)
            entity.ToTable("User");

            // One-to-One relationship configuration with Customer
            entity.HasOne(u => u.Customer)
                .WithOne(c => c.User)
                .HasForeignKey<User>(u => u.CustomerId);

            // Property configurations (adjust data types and lengths as needed)
            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.CustomerId)
                .HasMaxLength(20)
                .HasColumnName("CustomerID");
            entity.Property(e => e.Password).HasMaxLength(100); // Increased length for security (use password hashing!)
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Role).HasDefaultValue(0);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
