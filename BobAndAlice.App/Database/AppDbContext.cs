using BobAndAlice.App.Entities;
using Microsoft.EntityFrameworkCore;

namespace BobAndAlice.App.Database
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<UserKey> UserKeys { get; set; }
    }
}
