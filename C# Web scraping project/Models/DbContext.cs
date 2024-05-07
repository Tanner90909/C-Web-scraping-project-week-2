using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using C__Web_scraping_project.Models;
using Microsoft.EntityFrameworkCore;

namespace CSharp_Web_scraping_project.Models
{
    public class CardDbContext : DbContext
    {
        public DbSet<Card>? tblCardDetails { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=(local);Initial Catalog=OPTCGWebScrapeDB;Integrated Security=True;TrustServerCertificate=true;");
            }
        }
    }

    public class UserDbContext : DbContext
    {
        public DbSet<User>? tblUsers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=(local);Initial Catalog=OPTCGWebScrapeDB;Integrated Security=True;TrustServerCertificate=true;");
            }
        }
    }
}