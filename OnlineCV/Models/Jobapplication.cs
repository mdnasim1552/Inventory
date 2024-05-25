using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OnlineCV.Models;

[Table("jobapplication")]
public partial class Jobapplication
{
    [Key]
    [Column("id")]
    [StringLength(50)]
    public string Id { get; set; } = null!;

    [Column("fullname")]
    public string? Fullname { get; set; }

    [Column("fathername")]
    public string? Fathername { get; set; }

    [Column("email")]
    public string? Email { get; set; }

    [Column("mobile")]
    public string? Mobile { get; set; }

    [Column("address")]
    public string? Address { get; set; }

    [Column("gender")]
    public string? Gender { get; set; }

    [Column("region")]
    public string? Region { get; set; }

    [Column("declaration")]
    public string? Declaration { get; set; }

    [Column("interest")]
    public string? Interest { get; set; }

    [Column("photo_url")]
    public string? PhotoUrl { get; set; }

    [InverseProperty("Jobapplication")]
    public virtual ICollection<EducationalQualification> EducationalQualifications { get; set; } = new List<EducationalQualification>();
}
