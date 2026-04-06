using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PersonalWebsite.Api.Models;

[Keyless]
public partial class File
{
    [Column("ID")]
    [StringLength(50)]
    public string? Id { get; set; }

    public string? FileName { get; set; }

    [Column("File")]
    public byte[]? File1 { get; set; }
}
