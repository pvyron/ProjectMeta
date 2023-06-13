using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProMe.DataAccess.Models;
public class Contact
{
    [Key]
    public Guid Id { get; set; }
    [Required]
    [MaxLength(350)]
    public string Name { get; set; } = null!;
    [MaxLength(25)]
    public string? PhoneNumber { get; set; }
    [MaxLength(254)]
    public string? Email { get; set; }
}
