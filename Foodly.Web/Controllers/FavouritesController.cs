using System.Linq;
using System.Threading.Tasks;
using Foodly.Infrastructure.Data;
using Foodly.Web.Models.Favourites;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Foodly.Web.Controllers
{
    [Authorize]
    [Route("favourites")]
    public class FavouritesController : Controller
    {
        private readonly AppDbContext _db;
        private readonly UserManager<IdentityUser> _users;

        public FavouritesController(AppDbContext db, UserManager<IdentityUser> users)
        {
            _db = db;
            _users = users;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var uid = _users.GetUserId(User);

            // Берём любимые ПРОДУКТЫ (у твоего Favorite нет RestaurantId)
            var favProductIds = await _db.Favorites
                .Where(f => f.UserId == uid && f.ProductId != 0)
                .Select(f => f.ProductId)
                .ToListAsync();

            var products = await _db.Products
                .Where(p => favProductIds.Contains(p.Id))
                .Select(p => new { p.Id, p.Name, p.CategoryId, p.Price })
                .ToListAsync();

            var categories = await _db.Categories
                .Where(c => products.Select(p => p.CategoryId).Distinct().Contains(c.Id))
                .ToDictionaryAsync(c => c.Id, c => c.Name);

            // Просто возьмём топ ресторанов для вкладки "Restaurants"
            var restaurants = await _db.Restaurants
                .OrderBy(r => r.Id)
                .Take(6)
                .Select(r => new { r.Id, r.Name })
                .ToListAsync();

            var vm = new FavouritesVm
            {
                Dishes = products.Select(p => new FavouriteDishVm
                {
                    Id = p.Id,
                    Title = p.Name,
                    Image = $"/img/foods/{(p.Id % 6 + 1)}.jpg",           // плейсхолдер (положи картинки 1..6.jpg)
                    Meta = $"{(categories.TryGetValue(p.CategoryId, out var cat) ? cat : "Food")} • 20–30 min",
                    Badge = (p.Price % 2 == 0 ? "Free delivery" : $"{(p.Price % 5) + 1}$ Delivery")
                }).ToList(),

                Restaurants = restaurants.Select(r => new FavouriteRestaurantVm
                {
                    Id = r.Id,
                    Title = r.Name,
                    Logo = $"/img/brands/{(r.Id % 6 + 1)}.png",           // плейсхолдер (1..6.png)
                    Meta = $"{(r.Id % 2 == 0 ? "Burger" : "Pizza")} • {(r.Id % 40) / 2.0:0.#} km",
                    Badge = (r.Id % 3 == 0 ? "Buy 2 get 1 free" : "Free delivery")
                }).ToList()
            };

            return View(vm);
        }
    }
}
