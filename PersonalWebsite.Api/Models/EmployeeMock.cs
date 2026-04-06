using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PersonalWebsite.Api.Models;

[Table("EmployeeMock")]
public partial class EmployeeMock
{
    [Key]
    [Column("ID")]
    public int Id { get; set; }

    [Column("DepartmentID")]
    public int? DepartmentId { get; set; }

    [Column("AddressTypeID")]
    public int? AddressTypeId { get; set; }

    [Column("ContactTypeID")]
    public int? ContactTypeId { get; set; }

    [Column("EmailAddressID")]
    public int? EmailAddressId { get; set; }
}
