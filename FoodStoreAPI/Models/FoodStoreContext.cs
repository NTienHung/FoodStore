using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace FoodStoreAPI.Models
{
    public partial class FoodStoreContext : DbContext
    {
        public static FoodStoreContext Ins = new FoodStoreContext();
        public FoodStoreContext()
        {
            if(Ins != null)
            {
                Ins = this;
            }
        }

        public FoodStoreContext(DbContextOptions<FoodStoreContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Account> Accounts { get; set; } = null!;
        public virtual DbSet<AccountShipped> AccountShippeds { get; set; } = null!;
        public virtual DbSet<Category> Categories { get; set; } = null!;
        public virtual DbSet<Messagess> Messagesses { get; set; } = null!;
        public virtual DbSet<Order> Orders { get; set; } = null!;
        public virtual DbSet<OrderItem> OrderItems { get; set; } = null!;
        public virtual DbSet<Product> Products { get; set; } = null!;
        public virtual DbSet<ProductReview> ProductReviews { get; set; } = null!;
        public virtual DbSet<Revenue> Revenues { get; set; } = null!;
        public virtual DbSet<Role> Roles { get; set; } = null!;
        public virtual DbSet<Token> Tokens { get; set; } = null!;
        public virtual DbSet<Transaction> Transactions { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

            if (!optionsBuilder.IsConfigured) { optionsBuilder.UseSqlServer(config.GetConnectionString("value")); }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>(entity =>
            {
                entity.HasKey(e => e.AccId)
                    .HasName("PK__Account__91CBC398E9C00578");

                entity.ToTable("Account");

                entity.HasIndex(e => e.Username, "UC_Username")
                    .IsUnique();

                entity.Property(e => e.AccId).HasColumnName("AccID");

                entity.Property(e => e.Address).HasMaxLength(50);

                entity.Property(e => e.CreateAt)
                    .HasColumnType("datetime")
                    .HasColumnName("create_at");

                entity.Property(e => e.Email)
                    .HasMaxLength(30)
                    .HasColumnName("email");

                entity.Property(e => e.Name)
                    .HasMaxLength(30)
                    .HasColumnName("name");

                entity.Property(e => e.Password).HasColumnName("password");

                entity.Property(e => e.Phone)
                    .HasMaxLength(10)
                    .HasColumnName("phone");

                entity.Property(e => e.RoleId).HasColumnName("RoleID");

                entity.Property(e => e.UpdateAt)
                    .HasColumnType("datetime")
                    .HasColumnName("update_at");

                entity.Property(e => e.Username)
                    .HasMaxLength(50)
                    .HasColumnName("username");

                entity.Property(e => e.Wallet).HasColumnName("wallet");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.Accounts)
                    .HasForeignKey(d => d.RoleId)
                    .HasConstraintName("FK__Account__RoleID__3A81B327");
            });

            modelBuilder.Entity<AccountShipped>(entity =>
            {
                entity.HasKey(e => e.AccId)
                    .HasName("PK__AccountS__91CBC39822F5C738");

                entity.ToTable("AccountShipped");

                entity.Property(e => e.AccId)
                    .ValueGeneratedNever()
                    .HasColumnName("AccID");

                entity.Property(e => e.CreateAt)
                    .HasColumnType("datetime")
                    .HasColumnName("create_at");

                entity.Property(e => e.Status).HasMaxLength(30);

                entity.Property(e => e.UpdateAt)
                    .HasColumnType("datetime")
                    .HasColumnName("update_at");

                entity.HasOne(d => d.Acc)
                    .WithOne(p => p.AccountShipped)
                    .HasForeignKey<AccountShipped>(d => d.AccId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__AccountSh__AccID__3B75D760");

                entity.HasOne(d => d.Trans)
                    .WithMany(p => p.AccountShippeds)
                    .HasForeignKey(d => d.TransId)
                    .HasConstraintName("FK__AccountSh__Trans__3C69FB99");
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(e => e.CateId)
                    .HasName("PK__Category__27638D74A4A3292D");

                entity.ToTable("Category");

                entity.Property(e => e.CateId).HasColumnName("CateID");

                entity.Property(e => e.CateName).HasMaxLength(30);
            });

            modelBuilder.Entity<Messagess>(entity =>
            {
                entity.HasKey(e => e.MessId)
                    .HasName("PK__Messages__8DF8EF3FA786AEF7");

                entity.ToTable("Messagess");

                entity.Property(e => e.MessId).HasColumnName("messId");

                entity.Property(e => e.MessageText)
                    .HasMaxLength(500)
                    .HasColumnName("messageText");

                entity.Property(e => e.SentTime)
                    .HasColumnType("datetime")
                    .HasColumnName("sentTime");

                entity.HasOne(d => d.FromUser)
                    .WithMany(p => p.MessagessFromUsers)
                    .HasForeignKey(d => d.FromUserId)
                    .HasConstraintName("FK__Messagess__FromU__3D5E1FD2");

                entity.HasOne(d => d.ToUser)
                    .WithMany(p => p.MessagessToUsers)
                    .HasForeignKey(d => d.ToUserId)
                    .HasConstraintName("FK__Messagess__ToUse__3E52440B");
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.Property(e => e.OrderId).HasColumnName("OrderID");

                entity.Property(e => e.CreateAt)
                    .HasColumnType("datetime")
                    .HasColumnName("create_at");

                entity.Property(e => e.CustomerId).HasColumnName("CustomerID");

                entity.Property(e => e.OrderDate).HasColumnType("datetime");

                entity.Property(e => e.Status).HasMaxLength(30);

                entity.Property(e => e.UpdateAt)
                    .HasColumnType("datetime")
                    .HasColumnName("update_at");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.CustomerId)
                    .HasConstraintName("FK__Orders__Customer__412EB0B6");
            });

            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.Property(e => e.CreateAt)
                    .HasColumnType("datetime")
                    .HasColumnName("create_at");

                entity.Property(e => e.OrderId).HasColumnName("OrderID");

                entity.Property(e => e.ProductId).HasColumnName("ProductID");

                entity.Property(e => e.UpdateAt)
                    .HasColumnType("datetime")
                    .HasColumnName("update_at");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.OrderItems)
                    .HasForeignKey(d => d.OrderId)
                    .HasConstraintName("FK__OrderItem__Order__3F466844");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.OrderItems)
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("FK__OrderItem__Produ__403A8C7D");
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(e => e.ProId)
                    .HasName("PK__Product__620295F069505F05");

                entity.ToTable("Product");

                entity.Property(e => e.ProId).HasColumnName("ProID");

                entity.Property(e => e.AccId).HasColumnName("AccID");

                entity.Property(e => e.CateId).HasColumnName("CateID");

                entity.Property(e => e.CreateAt)
                    .HasColumnType("datetime")
                    .HasColumnName("create_at");

                entity.Property(e => e.DiscountEndTime).HasColumnType("datetime");

                entity.Property(e => e.DiscountPercentage).HasColumnType("decimal(18, 0)");

                entity.Property(e => e.DiscountStartTime).HasColumnType("datetime");

                entity.Property(e => e.Images).HasColumnName("images");

                entity.Property(e => e.Name).HasMaxLength(30);

                entity.Property(e => e.Price).HasColumnType("money");

                entity.Property(e => e.ProductStatus)
                    .HasMaxLength(20)
                    .HasColumnName("productStatus");

                entity.Property(e => e.Quantity).HasColumnName("quantity");

                entity.Property(e => e.Unit).HasMaxLength(30);

                entity.Property(e => e.UpdateAt)
                    .HasColumnType("datetime")
                    .HasColumnName("update_at");

                entity.HasOne(d => d.Acc)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.AccId)
                    .HasConstraintName("FK__Product__AccID__4222D4EF");

                entity.HasOne(d => d.Cate)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.CateId)
                    .HasConstraintName("FK__Product__CateID__4316F928");
            });

            modelBuilder.Entity<ProductReview>(entity =>
            {
                entity.HasKey(e => e.ReviewId)
                    .HasName("PK__ProductR__74BC79AE93892A91");

                entity.Property(e => e.ReviewId).HasColumnName("ReviewID");

                entity.Property(e => e.AccId).HasColumnName("AccID");

                entity.Property(e => e.CreateAt)
                    .HasColumnType("datetime")
                    .HasColumnName("create_at");

                entity.Property(e => e.Images).HasColumnName("images");

                entity.Property(e => e.ProId).HasColumnName("ProID");

                entity.Property(e => e.Star).HasColumnName("star");

                entity.Property(e => e.UpdateAt)
                    .HasColumnType("datetime")
                    .HasColumnName("update_at");

                entity.HasOne(d => d.Acc)
                    .WithMany(p => p.ProductReviews)
                    .HasForeignKey(d => d.AccId)
                    .HasConstraintName("FK__ProductRe__AccID__440B1D61");

                entity.HasOne(d => d.Pro)
                    .WithMany(p => p.ProductReviews)
                    .HasForeignKey(d => d.ProId)
                    .HasConstraintName("FK__ProductRe__ProID__44FF419A");
            });

            modelBuilder.Entity<Revenue>(entity =>
            {
                entity.ToTable("Revenue");

                entity.Property(e => e.RevenueId).HasColumnName("revenueId");

                entity.Property(e => e.AccId).HasColumnName("AccID");

                entity.Property(e => e.CreateAt)
                    .HasColumnType("datetime")
                    .HasColumnName("create_At");

                entity.Property(e => e.InterestRate).HasColumnName("interestRate");

                entity.Property(e => e.RevenuePrice).HasColumnName("revenuePrice");

                entity.HasOne(d => d.Acc)
                    .WithMany(p => p.Revenues)
                    .HasForeignKey(d => d.AccId)
                    .HasConstraintName("FK__Revenue__AccID__45F365D3");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("Role");

                entity.Property(e => e.RoleId).HasColumnName("RoleID");

                entity.Property(e => e.RoleName).HasMaxLength(20);
            });

            modelBuilder.Entity<Token>(entity =>
            {
                entity.ToTable("Token");

                entity.Property(e => e.AccId).HasColumnName("AccID");

                entity.Property(e => e.CreateAt)
                    .HasColumnType("datetime")
                    .HasColumnName("create_at");

                entity.Property(e => e.ExpirationDate).HasColumnType("datetime");

                entity.Property(e => e.TokenString).HasColumnName("tokenString");

                entity.HasOne(d => d.Acc)
                    .WithMany(p => p.Tokens)
                    .HasForeignKey(d => d.AccId)
                    .HasConstraintName("FK__Token__AccID__46E78A0C");
            });

            modelBuilder.Entity<Transaction>(entity =>
            {
                entity.HasKey(e => e.TransId)
                    .HasName("PK__Transact__9E5DDB3C9AFC48F0");

                entity.Property(e => e.AccId).HasColumnName("AccID");

                entity.Property(e => e.Amount).HasColumnType("decimal(18, 0)");

                entity.Property(e => e.ProId).HasColumnName("ProID");

                entity.Property(e => e.Status).HasMaxLength(30);

                entity.Property(e => e.TransDate).HasColumnType("datetime");

                entity.HasOne(d => d.Acc)
                    .WithMany(p => p.Transactions)
                    .HasForeignKey(d => d.AccId)
                    .HasConstraintName("FK__Transacti__AccID__47DBAE45");

                entity.HasOne(d => d.Pro)
                    .WithMany(p => p.Transactions)
                    .HasForeignKey(d => d.ProId)
                    .HasConstraintName("FK__Transacti__ProID__48CFD27E");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
