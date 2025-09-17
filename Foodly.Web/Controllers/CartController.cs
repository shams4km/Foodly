using Foodly.Infrastructure.Data;
using Foodly.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Foodly.Web.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly AppDbContext _ctx;
        public CartController(AppDbContext ctx) => _ctx = ctx;

        // GET /Cart
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var uid = User.Identity!.Name!;
            // join CartItems -> Products (в случае, если у CartItem нет навигационного свойства Product)
            var items = await _ctx.CartItems
                .Where(c => c.UserId == uid)
                .Join(_ctx.Products,
                      c => c.ProductId,
                      p => p.Id,
                      (c, p) => new CartItemVm
                      {
                          Id = c.Id,
                          ProductId = p.Id,
                          ProductName = p.Name,
                          UnitPrice = c.UnitPrice > 0 ? c.UnitPrice : p.Price,
                          Quantity = c.Quantity
                      })
                .ToListAsync();

            return View(items);
        }

        // POST /Cart/UpdateQty
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateQty(int id, int qty)
        {
            var it = await _ctx.CartItems.FindAsync(id);
            if (it == null) return NotFound();
            it.Quantity = Math.Max(1, qty);
            await _ctx.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // POST /Cart/Remove
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove(int id)
        {
            var it = await _ctx.CartItems.FindAsync(id);
            if (it == null) return NotFound();
            _ctx.CartItems.Remove(it);
            await _ctx.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
