using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Facturon.Domain.Entities;
using System;

namespace Facturon.Data
{
    public class FacturonDbContext : DbContext
    {
        public FacturonDbContext(DbContextOptions<FacturonDbContext> options) : base(options)
        {
        }

        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<InvoiceItem> InvoiceItems { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<TaxRate> TaxRates { get; set; }
        public DbSet<PaymentMethod> PaymentMethods { get; set; }
        public DbSet<Unit> Units { get; set; }
        public DbSet<ProductGroup> ProductGroups { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Invoice>(entity =>
            {
                entity.ToTable("Invoices");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Number)
                    .IsRequired();
                entity.HasIndex(e => e.Number).IsUnique();

                entity.HasOne(e => e.Supplier)
                    .WithMany(s => s.Invoices)
                    .HasForeignKey(e => e.SupplierId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired();

                entity.HasOne(e => e.PaymentMethod)
                    .WithMany(p => p.Invoices)
                    .HasForeignKey(e => e.PaymentMethodId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired();

                entity.Property(e => e.IsGrossBased)
                    .HasDefaultValue(false);

                entity.Property(e => e.DateCreated)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.DateUpdated)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.Active)
                    .HasDefaultValue(true);

                entity.HasIndex(e => e.SupplierId);
                entity.HasIndex(e => e.PaymentMethodId);
            });

            modelBuilder.Entity<InvoiceItem>(entity =>
            {
                entity.ToTable("InvoiceItems");
                entity.HasKey(e => e.Id);

                entity.HasOne(e => e.Invoice)
                    .WithMany(i => i.Items)
                    .HasForeignKey(e => e.InvoiceId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();

                entity.HasOne(e => e.Product)
                    .WithMany(p => p.InvoiceItems)
                    .HasForeignKey(e => e.ProductId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired();

                entity.HasOne(e => e.TaxRate)
                    .WithMany()
                    .HasForeignKey(e => e.TaxRateId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired();

                entity.Property(e => e.DateCreated)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.DateUpdated)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.Active)
                    .HasDefaultValue(true);

                entity.Property(e => e.TaxRateValue)
                    .HasColumnType("decimal(18,2)")
                    .HasDefaultValue(0m)
                    .IsRequired();

                entity.HasIndex(e => e.InvoiceId);
                entity.HasIndex(e => e.ProductId);
                entity.HasIndex(e => e.TaxRateId);
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("Products");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Name)
                    .IsRequired();
                entity.HasIndex(e => e.Name).IsUnique();

                entity.HasOne(e => e.Unit)
                    .WithMany(u => u.Products)
                    .HasForeignKey(e => e.UnitId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired();

                entity.HasOne(e => e.ProductGroup)
                    .WithMany(g => g.Products)
                    .HasForeignKey(e => e.ProductGroupId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired();

                entity.HasOne(e => e.TaxRate)
                    .WithMany(t => t.Products)
                    .HasForeignKey(e => e.TaxRateId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired();

                entity.Property(e => e.NetUnitPrice)
                    .HasColumnType("decimal(18,2)")
                    .HasDefaultValue(0m);

                entity.Property(e => e.DateCreated)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.DateUpdated)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.Active)
                    .HasDefaultValue(true);

                entity.HasIndex(e => e.UnitId);
                entity.HasIndex(e => e.ProductGroupId);
                entity.HasIndex(e => e.TaxRateId);
            });

            modelBuilder.Entity<Supplier>(entity =>
            {
                entity.ToTable("Suppliers");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Name)
                    .IsRequired();

                entity.Property(e => e.TaxNumber)
                    .IsRequired();
                entity.HasIndex(e => e.TaxNumber).IsUnique();

                entity.Property(e => e.Address)
                    .IsRequired();

                entity.Property(e => e.DateCreated)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.DateUpdated)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.Active)
                    .HasDefaultValue(true);
            });

            modelBuilder.Entity<TaxRate>(entity =>
            {
                entity.ToTable("TaxRates");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Code)
                    .IsRequired();
                entity.HasIndex(e => e.Code).IsUnique();

                entity.Property(e => e.Value)
                    .HasColumnType("decimal(18,2)");

                entity.Property(e => e.DateCreated)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.DateUpdated)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.Active)
                    .HasDefaultValue(true);
            });

            modelBuilder.Entity<PaymentMethod>(entity =>
            {
                entity.ToTable("PaymentMethods");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Name)
                    .IsRequired();
                entity.HasIndex(e => e.Name).IsUnique();

                entity.Property(e => e.DateCreated)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.DateUpdated)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.Active)
                    .HasDefaultValue(true);
            });

            modelBuilder.Entity<Unit>(entity =>
            {
                entity.ToTable("Units");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Name)
                    .IsRequired();
                entity.Property(e => e.ShortName)
                    .IsRequired();
                entity.HasIndex(e => e.ShortName).IsUnique();

                entity.Property(e => e.DateCreated)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.DateUpdated)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.Active)
                    .HasDefaultValue(true);
            });

            modelBuilder.Entity<ProductGroup>(entity =>
            {
                entity.ToTable("ProductGroups");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Name)
                    .IsRequired();
                entity.HasIndex(e => e.Name).IsUnique();

                entity.Property(e => e.DateCreated)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.DateUpdated)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.Active)
                    .HasDefaultValue(true);
            });
        }
    }
}
