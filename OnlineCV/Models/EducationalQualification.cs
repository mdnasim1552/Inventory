using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OnlineCV.Models;

[Table("educational_qualification")]
public partial class EducationalQualification
{
    [Key]
    [Column("Edu_id")]
    public int EduId { get; set; }

    [Column("exam")]
    public string? Exam { get; set; }

    [Column("board")]
    public string? Board { get; set; }

    [Column("year")]
    public string? Year { get; set; }

    [Column("result")]
    public string? Result { get; set; }

    [Column("jobapplication_id")]
    [StringLength(50)]
    public string JobapplicationId { get; set; } = null!;

    [ForeignKey("JobapplicationId")]
    [InverseProperty("EducationalQualifications")]
    public virtual Jobapplication Jobapplication { get; set; } = null!;
}
