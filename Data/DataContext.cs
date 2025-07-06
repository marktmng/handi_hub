using Microsoft.EntityFrameworkCore;
using DotnetAPI.Models;
using DotnetAPI.Dtos;

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
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; } // Add Order entity
        public DbSet<OrderItem> OrderItems { get; set; } // Add OrderItem entity
        public DbSet<CartItem> CartItems { get; set; } // Add CartItem entity for shopping cart functionality

        // Represents the "Dtos" for read-only queries
        public DbSet<ArtistDto> ArtistDtos { get; set; }
        public DbSet<UsersDto> UserDtos { get; set; }
        public DbSet<CustomerDto> CustomerDtos { get; set; }
        public DbSet<CategoryDto> CategoryDtos { get; set; }
        public DbSet<ProductDto> ProductDtos { get; set; }
        public DbSet<OrderDto> OrderDtos { get; set; } // Add OrderDto for read-only queries
        public DbSet<OrderItemDto> OrderItemDtos { get; set; } // Add OrderItemDto for read-only queries
        public DbSet<CartItemDto> CartItemDtos { get; set; } // Add CartItemDto for shopping cart functionality


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

            // Configure the Customer entity
            modelBuilder.Entity<Customer>()
                .ToTable("Customers", "HandiHub")
                .HasKey(c => c.CustomerId);

            // Configure the Category entity
            modelBuilder.Entity<Category>()
                .ToTable("Categories", "HandiHub")
                .HasKey(c => c.CategoryId);

            // Configure the Product entity
            modelBuilder.Entity<Product>()
                .ToTable("Products", "HandiHub")
                .HasKey(p => p.ProductId);

            // Configure the Order entity
            modelBuilder.Entity<Order>()
                .ToTable("Orders", "HandiHub")
                .HasKey(o => o.OrderId);

            // Configure the OrderItem entity
            modelBuilder.Entity<OrderItem>()
                .ToTable("OrderItems", "HandiHub")
                .HasKey(oi => oi.OrderItemId);

            // Configure the CartItem entity
            modelBuilder.Entity<CartItem>()
                .ToTable("CartItems", "HandiHub")
                .HasKey(ci => new { ci.CartId, ci.UserId, ci.ProductId }); // Composite key

            // Configure the Dto entity (no key, used for read-only queries)
            modelBuilder.Entity<ArtistDto>().HasNoKey();
            modelBuilder.Entity<UsersDto>().HasNoKey();
            modelBuilder.Entity<CustomerDto>().HasNoKey();
            modelBuilder.Entity<CategoryDto>().HasNoKey();
            modelBuilder.Entity<ProductDto>().HasNoKey();
            modelBuilder.Entity<OrderDto>().HasNoKey();
            modelBuilder.Entity<OrderItemDto>().HasNoKey();
            modelBuilder.Entity<CartItemDto>().HasNoKey();
        }
    }
}
