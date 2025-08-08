using Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Service.DBContexts
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Edit> Edits { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
    } 
}
