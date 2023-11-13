using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace pharma_sales_and_management_system.Models
{
    public partial class pharma_managementContext : DbContext
    {
        public pharma_managementContext()
        {
        }

        public pharma_managementContext(DbContextOptions<pharma_managementContext> options)
            : base(options)
        {
        }

        public virtual DbSet<AgencyDetail> AgencyDetails { get; set; } = null!;
        public virtual DbSet<AgencyOrder> AgencyOrders { get; set; } = null!;
        public virtual DbSet<AgencyProductStock> AgencyProductStocks { get; set; } = null!;
        public virtual DbSet<Feedback> Feedbacks { get; set; } = null!;
        public virtual DbSet<Manufacturer> Manufacturers { get; set; } = null!;
        public virtual DbSet<MedicalOrder> MedicalOrders { get; set; } = null!;
        public virtual DbSet<MedicalSellingProduct> MedicalSellingProducts { get; set; } = null!;
        public virtual DbSet<MedicalShopDetail> MedicalShopDetails { get; set; } = null!;
        public virtual DbSet<MedicalShopProductStock> MedicalShopProductStocks { get; set; } = null!;
        public virtual DbSet<ProductCategory> ProductCategories { get; set; } = null!;
        public virtual DbSet<ProductDetail> ProductDetails { get; set; } = null!;
        public virtual DbSet<UserCart> UserCarts { get; set; } = null!;
        public virtual DbSet<UserDetail> UserDetails { get; set; } = null!;
        public virtual DbSet<UserOrder> UserOrders { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                ConfigurationBuilder confBuilder = new ConfigurationBuilder();
                optionsBuilder.UseSqlServer(confBuilder.Build().GetSection("ConnectionStrings:DBConnectionStrings").Value);
                //optionsBuilder.UseSqlServer("Data Source=LAPTOP-HU338L8D\\SQLEXPRESS01;Initial Catalog=pharma_management;Integrated Security=True;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AgencyDetail>(entity =>
            {
                entity.ToTable("agency_details");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.AgencyName)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("agency_name");

                entity.Property(e => e.ContactNo).HasColumnName("contact_no");

                entity.Property(e => e.Email)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("email");

                entity.Property(e => e.Password)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("password");

                entity.Property(e => e.ProfileImage)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("profile_image");
            });

            modelBuilder.Entity<AgencyOrder>(entity =>
            {
                entity.ToTable("agency_order");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.AgencyId).HasColumnName("agency_id");

                entity.Property(e => e.CompanyId).HasColumnName("company_id");

                entity.Property(e => e.Date)
                    .HasColumnType("date")
                    .HasColumnName("date");

                entity.Property(e => e.IsDelivered).HasColumnName("is_delivered");

                entity.Property(e => e.ProductId).HasColumnName("product_id");

                entity.Property(e => e.Quantity).HasColumnName("quantity");

                entity.HasOne(d => d.Agency)
                    .WithMany(p => p.AgencyOrders)
                    .HasForeignKey(d => d.AgencyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_agency");

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.AgencyOrders)
                    .HasForeignKey(d => d.CompanyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_company2");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.AgencyOrders)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_product4");
            });

            modelBuilder.Entity<AgencyProductStock>(entity =>
            {
                entity.ToTable("agency_product_stock");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.TotalQuantity).HasColumnName("total_quantity");
            });

            modelBuilder.Entity<Feedback>(entity =>
            {
                entity.ToTable("feedback");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Description)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("description");

                entity.Property(e => e.MedicalShopId).HasColumnName("medical_shop_id");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.HasOne(d => d.MedicalShop)
                    .WithMany(p => p.Feedbacks)
                    .HasForeignKey(d => d.MedicalShopId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_shop5");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Feedbacks)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_user4");
            });

            modelBuilder.Entity<Manufacturer>(entity =>
            {
                entity.ToTable("manufacturer");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.City)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("city");

                entity.Property(e => e.ComponyName)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("compony_name");

                entity.Property(e => e.ContactNo).HasColumnName("contact_no");

                entity.Property(e => e.Email)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("email");

                entity.Property(e => e.Password)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("password");
            });

            modelBuilder.Entity<MedicalOrder>(entity =>
            {
                entity.ToTable("medical_order");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.BillAmount).HasColumnName("bill_amount");

                entity.Property(e => e.CompanyId).HasColumnName("company_id");

                entity.Property(e => e.IsPlaced).HasColumnName("is_placed");

                entity.Property(e => e.MedicalShopId).HasColumnName("medical_shop_id");

                entity.Property(e => e.OrderDate)
                    .HasColumnType("date")
                    .HasColumnName("order_date");

                entity.Property(e => e.ProductId).HasColumnName("product_id");

                entity.Property(e => e.TotalQuantity).HasColumnName("total_quantity");

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.MedicalOrders)
                    .HasForeignKey(d => d.CompanyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_company4");

                entity.HasOne(d => d.MedicalShop)
                    .WithMany(p => p.MedicalOrders)
                    .HasForeignKey(d => d.MedicalShopId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_shop4");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.MedicalOrders)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_product6");
            });

            modelBuilder.Entity<MedicalSellingProduct>(entity =>
            {
                entity.ToTable("medical_selling_product");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.MedicalShopId).HasColumnName("medical_shop_id");

                entity.Property(e => e.Mrp).HasColumnName("MRP");

                entity.Property(e => e.ProductId).HasColumnName("product_id");

                entity.HasOne(d => d.MedicalShop)
                    .WithMany(p => p.MedicalSellingProducts)
                    .HasForeignKey(d => d.MedicalShopId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_shop");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.MedicalSellingProducts)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_product2");
            });

            modelBuilder.Entity<MedicalShopDetail>(entity =>
            {
                entity.ToTable("medical_shop_detail");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.City)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("city");

                entity.Property(e => e.ContactNo).HasColumnName("contact_no");

                entity.Property(e => e.Email)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("email");

                entity.Property(e => e.OwnerName)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("owner_name");

                entity.Property(e => e.Password)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("password");

                entity.Property(e => e.ProfilePic)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("profile_pic");
            });

            modelBuilder.Entity<MedicalShopProductStock>(entity =>
            {
                entity.ToTable("medical_shop_product_stock");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.MedicalShopId).HasColumnName("medical_shop_id");

                entity.Property(e => e.ProductId).HasColumnName("product_id");

                entity.Property(e => e.TotalQuantity).HasColumnName("total_quantity");

                entity.HasOne(d => d.MedicalShop)
                    .WithMany(p => p.MedicalShopProductStocks)
                    .HasForeignKey(d => d.MedicalShopId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_medical_shop");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.MedicalShopProductStocks)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_product");
            });

            modelBuilder.Entity<ProductCategory>(entity =>
            {
                entity.ToTable("product_categories");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CategoryName)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("category_name");
            });

            modelBuilder.Entity<ProductDetail>(entity =>
            {
                entity.ToTable("product_details");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CategoryId).HasColumnName("category_id");

                entity.Property(e => e.CompanyId).HasColumnName("company_id");

                entity.Property(e => e.Description)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("description");

                entity.Property(e => e.Disease)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("disease");

                entity.Property(e => e.ExpDate)
                    .HasColumnType("date")
                    .HasColumnName("exp_date");

                entity.Property(e => e.MfgDate)
                    .HasColumnType("date")
                    .HasColumnName("mfg_date");

                entity.Property(e => e.ProductImage)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("product_image");

                entity.Property(e => e.ProductName)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("product_name");

                entity.Property(e => e.RetailPrice).HasColumnName("retail_price");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.ProductDetails)
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_category");

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.ProductDetails)
                    .HasForeignKey(d => d.CompanyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_company");
            });

            modelBuilder.Entity<UserCart>(entity =>
            {
                entity.ToTable("user_cart");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.MedicalShopId).HasColumnName("medical_shop_id");

                entity.Property(e => e.ProductId).HasColumnName("product_id");

                entity.Property(e => e.Quantity).HasColumnName("quantity");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.HasOne(d => d.MedicalShop)
                    .WithMany(p => p.UserCarts)
                    .HasForeignKey(d => d.MedicalShopId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_shop2");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.UserCarts)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_product3");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserCarts)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_user");
            });

            modelBuilder.Entity<UserDetail>(entity =>
            {
                entity.ToTable("user_details");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.City)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("city");

                entity.Property(e => e.ContactNo).HasColumnName("contact_no");

                entity.Property(e => e.Email)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("email");

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("name");

                entity.Property(e => e.Password)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("password");

                entity.Property(e => e.Pincode).HasColumnName("pincode");

                entity.Property(e => e.ProfilePic)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("profile_pic");
            });

            modelBuilder.Entity<UserOrder>(entity =>
            {
                entity.ToTable("user_order");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.IsDelivered).HasColumnName("is_delivered");

                entity.Property(e => e.MedicalShopId).HasColumnName("medical_shop_id");

                entity.Property(e => e.OrderAddress)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("order_address");

                entity.Property(e => e.OrderCity)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("order_city");

                entity.Property(e => e.OrderDate)
                    .HasColumnType("date")
                    .HasColumnName("order_date");

                entity.Property(e => e.ProductId).HasColumnName("product_id");

                entity.Property(e => e.Quantity).HasColumnName("quantity");

                entity.Property(e => e.TotalAmount).HasColumnName("total_amount");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.HasOne(d => d.MedicalShop)
                    .WithMany(p => p.UserOrders)
                    .HasForeignKey(d => d.MedicalShopId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_shop3");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.UserOrders)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_product5");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserOrders)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_user2");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
