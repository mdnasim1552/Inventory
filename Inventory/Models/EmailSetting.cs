using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Models;

[Index("From", Name = "UK_From_EmailSettings", IsUnique = true)]
public partial class EmailSetting
{
    [Key]
    public int Id { get; set; }

    [StringLength(100)]
    public string SecretKey { get; set; } = null!;

    [StringLength(100)]
    public string From { get; set; } = null!;

    [StringLength(100)]
    public string SmtpServer { get; set; } = null!;

    public int Port { get; set; }

    [Column("EnableSSL")]
    public bool EnableSsl { get; set; }
}
