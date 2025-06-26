using Microsoft.EntityFrameworkCore;
using DotnetAPI.Models;

namespace DotnetAPI.Data
{
    // This class manages your database connection and access
    public class DataContext : DbContext
    {
        private readonly IConfiguration _config; // To access configuration like connection string

        // Constructor with injected configuration
        public DataContext(IConfiguration config)
        {
            _config = config;
        }

        // Represents the "Users" table in the database
        public DbSet<User> Users { get; set; }
        public DbSet<Artist> Artists { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Use the connection string from appsettings.json
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(_config.GetConnectionString("HandiHubConnection"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("HandiHub");

            // Configure the User entity
            modelBuilder.Entity<User>()
                .ToTable("Users", "HandiHub")
                .HasKey(u => u.UserId);

            // Configure the Artist entity
            modelBuilder.Entity<Artist>()
                .ToTable("Artists", "HandiHub")
                .HasKey(a => a.ArtistId);
        }
    }
}
