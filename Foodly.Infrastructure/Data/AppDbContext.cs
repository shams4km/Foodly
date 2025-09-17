using Foodly.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Foodly.Infrastructure.Data
{
    public class AppDbContext : IdentityDbContext<IdentityUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Restaurant> Restaurants => Set<Restaurant>();
        public DbSet<Product> Products => Set<Product>();
        public DbSet<Favorite> Favorites => Set<Favorite>();
        public DbSet<CartItem> CartItems => Set<CartItem>();
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<OrderItem> OrderItems => Set<OrderItem>();
        public DbSet<SubscriptionPlan> SubscriptionPlans => Set<SubscriptionPlan>();
        public DbSet<UserSubscription> UserSubscriptions => Set<UserSubscription>();

        protected override void OnModelCreating(ModelBuilder b)
        {
            base.OnModelCreating(b);

            b.Entity<Category>().HasIndex(x => x.Slug).IsUnique();

            b.Entity<Favorite>()
                .HasIndex(x => new { x.UserId, x.ProductId })
                .IsUnique();

            b.Entity<CartItem>()
                .HasIndex(x => new { x.UserId, x.ProductId })
                .IsUnique();

            b.Entity<UserSubscription>()
                .HasIndex(x => new { x.UserId, x.IsActive });
        }
    }
}
