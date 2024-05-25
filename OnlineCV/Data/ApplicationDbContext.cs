using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using OnlineCV.Models;

namespace OnlineCV.Data;

public partial class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Credential> Credentials { get; set; }

    public virtual DbSet<EducationalQualification> EducationalQualifications { get; set; }

    public virtual DbSet<Jobapplication> Jobapplications { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Credential>(entity =>
        {
            entity.Property(e => e.Gender).IsFixedLength();
        });

        modelBuilder.Entity<EducationalQualification>(entity =>
        {
            entity.HasKey(e => e.EduId).HasName("PK__educatio__E547B02ADA2AF065");

            entity.HasOne(d => d.Jobapplication).WithMany(p => p.EducationalQualifications).HasConstraintName("FK__education__jobap__2E1BDC42");
        });

        modelBuilder.Entity<Jobapplication>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__jobappli__3213E83F82772CEE");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
