using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProMe.DataAccess.Models;

[Index(nameof(AccessKey), IsUnique = true)]
public class CampaignManager
{
    [Key]
    public Guid Id { get; set; }
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = null!;
    [Required]
    [MaxLength(254)]
    public string Email { get; set; } = null!;
    [Required]
    public string AccessKey { get; set; } = null!;
    [Required]
    public DateTimeOffset CreationDate { get; set; } = DateTimeOffset.UtcNow;
    [Required]
    public DateTimeOffset LastModifiedDate { get; set; } = DateTimeOffset.UtcNow;
    [Required]
    public DateTimeOffset LastKeyReset { get; set; } = DateTimeOffset.UtcNow;

    public virtual ICollection<Campaign> Campaigns { get; set; } = new HashSet<Campaign>();
}
