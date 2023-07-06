using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProMe.DataAccess.Models;

[Index(nameof(Email), IsUnique = true)]
public class User
{
    [Key]
    public Guid Id { get; set; }
    [Required]
    [MaxLength(254)]
    public string Email { get; set; } = null!;
    [Required]
    public string Key { get; set; } = null!;
    [Required]
    public string Salt { get; set; } = null!;
    [Required]
    public DateTimeOffset CreationDate { get; set; } = DateTimeOffset.UtcNow;
    [Required]
    public DateTimeOffset LastModifiedDate { get; set; } = DateTimeOffset.UtcNow;
    [Required]
    public DateTimeOffset LastPasswordReset { get; set; } = DateTimeOffset.UtcNow;
    [Required]
    public bool Verified { get; set; } = false;
    [ForeignKey(nameof(Contact))]
    public Guid? ContactId { get; set; }
    public Contact? Contact { get; set; }
}
