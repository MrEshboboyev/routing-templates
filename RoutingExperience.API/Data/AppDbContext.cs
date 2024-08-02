using Microsoft.EntityFrameworkCore;
using RoutingExperience.API.Models;

namespace RoutingExperience.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
    }
}
