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

    public virtual DbSet<Cart> Carts { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<ChatRoom> ChatRooms { get; set; }

    public virtual DbSet<ChatRoomMember> ChatRoomMembers { get; set; }

    public virtual DbSet<Credential> Credentials { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<EmailSetting> EmailSettings { get; set; }

    public virtual DbSet<Message> Messages { get; set; }

    public virtual DbSet<MessageStatus> MessageStatuses { get; set; }

    public virtual DbSet<PasswordResetToken> PasswordResetTokens { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductStore> ProductStores { get; set; }

    public virtual DbSet<Store> Stores { get; set; }

    public virtual DbSet<SubCategory> SubCategories { get; set; }

    public virtual DbSet<Supplier> Suppliers { get; set; }

    public virtual DbSet<Unit> Units { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserConnection> UserConnections { get; set; }

    public virtual DbSet<Userrole> Userroles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cart>(entity =>
        {
            entity.HasKey(e => e.CartId).HasName("PK__Cart__51BCD7B7363A083F");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Quantity).HasDefaultValue(1);
            entity.Property(e => e.Selected).HasDefaultValue(false, "DF_Cart_Selected");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Product).WithMany(p => p.Carts).HasConstraintName("FK_Cart_Product");

            entity.HasOne(d => d.User).WithMany(p => p.Carts).HasConstraintName("FK_Cart_User");
        });

        modelBuilder.Entity<ChatRoom>(entity =>
        {
            entity.HasKey(e => e.ChatRoomId).HasName("PK__ChatRoom__69733CF7A51DFC73");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
        });

        modelBuilder.Entity<ChatRoomMember>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ChatRoom__3214EC07075D28EB");

            entity.Property(e => e.JoinedAt).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.ChatRoom).WithMany(p => p.ChatRoomMembers).HasConstraintName("FK__ChatRoomM__ChatR__370627FE");

            entity.HasOne(d => d.User).WithMany(p => p.ChatRoomMembers).HasConstraintName("FK__ChatRoomM__UserI__37FA4C37");
        });

        modelBuilder.Entity<Credential>(entity =>
        {
            entity.Property(e => e.Gender).IsFixedLength();

            entity.HasOne(d => d.Role).WithMany(p => p.Credentials)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RoleId_Credential");

            entity.HasOne(d => d.Store).WithMany(p => p.Credentials).HasConstraintName("FK_Credential_Store");
        });

        modelBuilder.Entity<EmailSetting>(entity =>
        {
            entity.Property(e => e.From).IsFixedLength();
            entity.Property(e => e.SecretKey).IsFixedLength();
            entity.Property(e => e.SmtpServer).IsFixedLength();
        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity.HasKey(e => e.MessageId).HasName("PK__Messages__C87C0C9CBAD462CA");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.MessageType).HasDefaultValue("Text");

            entity.HasOne(d => d.ChatRoom).WithMany(p => p.Messages).HasConstraintName("FK__Messages__ChatRo__3BCADD1B");

            entity.HasOne(d => d.Sender).WithMany(p => p.Messages).HasConstraintName("FK__Messages__Sender__3CBF0154");
        });

        modelBuilder.Entity<MessageStatus>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__MessageS__3214EC07F270C3AE");

            entity.Property(e => e.IsSeen).HasDefaultValue(false);

            entity.HasOne(d => d.Message).WithMany(p => p.MessageStatuses).HasConstraintName("FK__MessageSt__Messa__4183B671");

            entity.HasOne(d => d.User).WithMany(p => p.MessageStatuses).HasConstraintName("FK__MessageSt__UserI__4277DAAA");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.Property(e => e.Discount).HasDefaultValue(0.00m, "DF_Product_Discount");
            entity.Property(e => e.Sku).IsFixedLength();
            entity.Property(e => e.Status).IsFixedLength();
            entity.Property(e => e.Tax).HasDefaultValue(0.00m, "DF_Product_Tax");

            entity.HasOne(d => d.Brand).WithMany(p => p.Products)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Product_Brand");

            entity.HasOne(d => d.Category).WithMany(p => p.Products)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Product_Category");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.Products)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Product_Credential");

            entity.HasOne(d => d.SubCategory).WithMany(p => p.Products)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Product_SubCategory");

            entity.HasOne(d => d.Unit).WithMany(p => p.Products)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Product_Unit");
        });

        modelBuilder.Entity<ProductStore>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ProductS__3214EC070CBE1085");

            entity.HasOne(d => d.Product).WithMany(p => p.ProductStores)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProductStore_Product");

            entity.HasOne(d => d.Store).WithMany(p => p.ProductStores)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProductStore_Store");
        });

        modelBuilder.Entity<Store>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Store__3214EC070AEF3F1E");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
        });

        modelBuilder.Entity<SubCategory>(entity =>
        {
            entity.HasOne(d => d.Category).WithMany(p => p.SubCategories)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SubCategory_Category");
        });

        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Supplier__3214EC0777A18D76");
        });

        modelBuilder.Entity<Unit>(entity =>
        {
            entity.Property(e => e.ShortName).IsFixedLength();
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Users__3214EC07CA49CA8E");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
        });

        modelBuilder.Entity<UserConnection>(entity =>
        {
            entity.HasKey(e => e.ConnectionId).HasName("PK__UserConn__404A649318DE4836");

            entity.Property(e => e.ConnectedAt).HasDefaultValueSql("(getdate())");
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
