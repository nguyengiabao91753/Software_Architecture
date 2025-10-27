using Auth.API.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Auth.API.Data
{
    public class IdentityApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public IdentityApplicationDbContext(DbContextOptions<IdentityApplicationDbContext> options) :
        base(options)
        { }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
