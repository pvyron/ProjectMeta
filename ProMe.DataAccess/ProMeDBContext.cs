using Microsoft.EntityFrameworkCore;
using ProMe.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProMe.DataAccess;
public class ProMeDBContext : DbContext
{
    public ProMeDBContext(DbContextOptions<ProMeDBContext> dbContextOptions) : base(dbContextOptions) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Contact> Contacts { get; set; }
}
