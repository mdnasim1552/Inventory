﻿using System;
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

    public virtual DbSet<Credential> Credentials { get; set; }

    public virtual DbSet<EducationalQualification> EducationalQualifications { get; set; }

    public virtual DbSet<EmailSetting> EmailSettings { get; set; }

    public virtual DbSet<Jobapplication> Jobapplications { get; set; }

    public virtual DbSet<Userrole> Userroles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Credential>(entity =>
        {
            entity.Property(e => e.Gender).IsFixedLength();

            entity.HasOne(d => d.Role).WithMany(p => p.Credentials).HasConstraintName("FK_RoleId_Credential");
        });

        modelBuilder.Entity<EducationalQualification>(entity =>
        {
            entity.HasKey(e => e.EduId).HasName("PK__educatio__E547B02ADA2AF065");

            entity.HasOne(d => d.Jobapplication).WithMany(p => p.EducationalQualifications).HasConstraintName("FK__education__jobap__2E1BDC42");
        });

        modelBuilder.Entity<EmailSetting>(entity =>
        {
            entity.Property(e => e.From).IsFixedLength();
            entity.Property(e => e.SecretKey).IsFixedLength();
            entity.Property(e => e.SmtpServer).IsFixedLength();
        });

        modelBuilder.Entity<Jobapplication>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__jobappli__3213E83F82772CEE");
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