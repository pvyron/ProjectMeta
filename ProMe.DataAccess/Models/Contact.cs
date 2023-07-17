using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

    [Required]
    [ForeignKey(nameof(User))]
    public Guid UserId { get; set; }
    public User? User { get; set; }

    public ICollection<Campaign> Campaigns { get; set; } = new HashSet<Campaign>();
}
