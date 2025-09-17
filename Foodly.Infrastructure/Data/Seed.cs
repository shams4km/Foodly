using Foodly.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Foodly.Infrastructure.Data
{
    public static class Seed
    {
        public static async Task RunAsync(IServiceProvider sp)
        {
            using var scope = sp.CreateScope();
            var ctx = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await ctx.Database.MigrateAsync();

            // Роли
            var roleMgr = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            foreach (var r in new[] { "Admin", "Owner", "Customer" })
                if (!await roleMgr.RoleExistsAsync(r))
                    await roleMgr.CreateAsync(new IdentityRole(r));

            // Админ-пользователь
            var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var email = "admin@foodly.local";
            var admin = await userMgr.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (admin == null)
            {
                admin = new IdentityUser { UserName = email, Email = email, EmailConfirmed = true };
                await userMgr.CreateAsync(admin, "Admin123!");
            }

            // Добавляем роль Admin, если её нет
            if (!await userMgr.IsInRoleAsync(admin, "Admin"))
                await userMgr.AddToRoleAsync(admin, "Admin");

            // Добавляем запись в UserSubscription (Pro) для админа
            if (!ctx.UserSubscriptions.Any(us => us.UserId == admin.Id))
            {
                // Берём Pro-план
                var proPlan = await ctx.SubscriptionPlans.FirstOrDefaultAsync(p => p.Level == 1);
                if (proPlan == null)
                {
                    // Если план ещё не создан, создаём его
                    proPlan = new SubscriptionPlan
                    {
                        Name = "Pro",
                        Level = 1,
                        PricePerMonth = 9.99m,
                        Features = "Favourites, Chat, Free delivery"
                    };
                    ctx.SubscriptionPlans.Add(proPlan);
                    await ctx.SaveChangesAsync();
                }

                ctx.UserSubscriptions.Add(new UserSubscription
                {
                    UserId = admin.Id,
                    PlanId = proPlan.Id,
                    StartDate = DateTime.UtcNow,
                    EndDate = DateTime.UtcNow.AddYears(1),
                    IsActive = true
                });
            }

            // Стартовые категории
            if (!ctx.Categories.Any())
            {
                ctx.Categories.AddRange(
                    new Category { Name = "Burgers", Slug = "burgers" },
                    new Category { Name = "Pizza",   Slug = "pizza" },
                    new Category { Name = "Sushi",   Slug = "sushi" }
                );
            }

            // Стартовые рестораны и продукты
            if (!ctx.Restaurants.Any())
            {
                var rest = new Restaurant { Name = "Sunrise Diner", Description = "Tasty burgers and fries." };
                ctx.Restaurants.Add(rest);
                await ctx.SaveChangesAsync();

                var burgersId = (await ctx.Categories.FirstAsync(c => c.Slug == "burgers")).Id;
                var pizzaId   = (await ctx.Categories.FirstAsync(c => c.Slug == "pizza")).Id;

                ctx.Products.AddRange(
                    new Product { Name = "Classic Burger", Price = 7.99m, RestaurantId = rest.Id, CategoryId = burgersId },
                    new Product { Name = "Cheese Pizza",   Price = 10.50m, RestaurantId = rest.Id, CategoryId = pizzaId }
                );
            }

            // Планы подписки
            if (!ctx.SubscriptionPlans.Any())
            {
                ctx.SubscriptionPlans.AddRange(
                    new SubscriptionPlan { Name = "Basic", Level = 0, PricePerMonth = 0, Features = "Limited features" },
                    new SubscriptionPlan { Name = "Pro",   Level = 1, PricePerMonth = 9.99m, Features = "Favourites, Chat, Free delivery" }
                );
            }

            await ctx.SaveChangesAsync();
        }
    }
}
