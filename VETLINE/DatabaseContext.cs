using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VETLINE.Models;

namespace VETLINE
{
    public class DatabaseContext : IdentityDbContext<ApplicationUser>
	{
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }

        public DatabaseContext(DbContextOptions options) : base(options)
        {
        }

		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);
		}
	}
}
