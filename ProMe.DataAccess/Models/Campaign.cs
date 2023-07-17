using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProMe.DataAccess.Models;

public class Campaign
{
    [Key]
    public Guid Id { get; set; }
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = null!;
    [Required]
    public string Description { get; set; } = null!;
    public DateTimeOffset ActiveFrom { get; set; } = DateTimeOffset.Now;
    public DateTimeOffset ActiveTo { get; set; } = DateTimeOffset.MaxValue;

    [Required]
    [ForeignKey(nameof(CampaignManager))]
    public Guid CampaignManagerId { get; set; }
    public virtual CampaignManager CampaignManager { get; set; } = null!;

    public virtual ICollection<Contact> Contacts { get; set; } = new HashSet<Contact>();
}
