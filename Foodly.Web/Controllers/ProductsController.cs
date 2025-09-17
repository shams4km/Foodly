using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Foodly.Infrastructure.Data;

namespace Foodly.Web.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class ProductsController : Controller
    {
        private readonly AppDbContext _ctx;

        public ProductsController(AppDbContext ctx)
        {
            _ctx = ctx;
        }

        // GET /Products/Details?id=1
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var product = await _ctx.Products
                .Where(p => p.Id == id)
                .Select(p => new
                {
                    p.Id,
                    p.Name,
                    p.Description,
                    p.Price,
                    Category = p.Category != null ? p.Category.Name : null,
                    Restaurant = p.Restaurant != null ? p.Restaurant.Name : null
                })
                .FirstOrDefaultAsync();

            if (product == null) return NotFound();

            return Json(product);
        }

        // POST /Products/AddToCart?id=1
        [HttpPost]
        public async Task<IActionResult> AddToCart(int id)
        {
            var user = User.Identity?.Name ?? "guest";

            var existing = await _ctx.CartItems
                .FirstOrDefaultAsync(c => c.UserId == user && c.ProductId == id);

            if (existing != null)
            {
                existing.Quantity++;
            }
            else
            {
                _ctx.CartItems.Add(new Foodly.Domain.Entities.CartItem
                {
                    UserId = user,
                    ProductId = id,
                    Quantity = 1,
                    UnitPrice = await _ctx.Products.Where(p => p.Id == id).Select(p => p.Price).FirstOrDefaultAsync()
                });
            }

            await _ctx.SaveChangesAsync();
            return Json(new { ok = true, qty = existing?.Quantity ?? 1 });
        }
    }
}
