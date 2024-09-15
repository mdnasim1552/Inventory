using System;
using System.Collections.Generic;
using Inventory.Models;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Data;

public partial class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Brand> Brands { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Credential> Credentials { get; set; }

    public virtual DbSet<EmailSetting> EmailSettings { get; set; }

    public virtual DbSet<PasswordResetToken> PasswordResetTokens { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<SubCategory> SubCategories { get; set; }

    public virtual DbSet<Unit> Units { get; set; }

    public virtual DbSet<Userrole> Userroles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Credential>(entity =>
        {
            entity.Property(e => e.Gender).IsFixedLength();

            entity.HasOne(d => d.Role).WithMany(p => p.Credentials)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RoleId_Credential");
        });

        modelBuilder.Entity<EmailSetting>(entity =>
        {
            entity.Property(e => e.From).IsFixedLength();
            entity.Property(e => e.SecretKey).IsFixedLength();
            entity.Property(e => e.SmtpServer).IsFixedLength();
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.Property(e => e.Discount).HasDefaultValue(0.00m);
            entity.Property(e => e.Sku).IsFixedLength();
            entity.Property(e => e.Status).IsFixedLength();
            entity.Property(e => e.Tax).HasDefaultValue(0.00m);

            entity.HasOne(d => d.Brand).WithMany(p => p.Products)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Product_Brand");

            entity.HasOne(d => d.Category).WithMany(p => p.Products)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Product_Category");

            entity.HasOne(d => d.SubCategory).WithMany(p => p.Products)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Product_SubCategory");

            entity.HasOne(d => d.Unit).WithMany(p => p.Products)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Product_Unit");
        });

        modelBuilder.Entity<SubCategory>(entity =>
        {
            entity.HasOne(d => d.Category).WithMany(p => p.SubCategories)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SubCategory_Category");
        });

        modelBuilder.Entity<Unit>(entity =>
        {
            entity.Property(e => e.ShortName).IsFixedLength();
        });

        modelBuilder.Entity<Userrole>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Userrole__006568E9FF2BC678");

            entity.Property(e => e.RoleId).ValueGeneratedNever();
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
