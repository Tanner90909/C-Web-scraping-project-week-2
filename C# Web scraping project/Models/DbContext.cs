using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                optionsBuilder.UseSqlServer("Server=(local);Initial Catalog=OPTCGWebScrapeDB;User ID=testuser;Password=testing;TrustServerCertificate=true;");
            }
        }
    }
}