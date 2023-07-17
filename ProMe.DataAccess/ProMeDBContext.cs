using Microsoft.EntityFrameworkCore;
using ProMe.DataAccess.Models;

namespace ProMe.DataAccess;
public class ProMeDBContext : DbContext
{
    public ProMeDBContext(DbContextOptions<ProMeDBContext> dbContextOptions) : base(dbContextOptions) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Contact> Contacts { get; set; }
    public DbSet<CampaignManager> CampaignManagers { get; set; }
}
