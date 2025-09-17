using Foodly.Infrastructure.Data;
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

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var uid = User.Identity!.Name!;
            var items = await _ctx.CartItems.Where(c => c.UserId == uid).ToListAsync();
            return Json(items);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateQty(int id, int qty)
        {
            var it = await _ctx.CartItems.FindAsync(id);
            if (it == null) return NotFound();
            it.Quantity = Math.Max(1, qty);
            await _ctx.SaveChangesAsync();
            return Json(new { ok = true, qty = it.Quantity });
        }

        [HttpDelete]
        public async Task<IActionResult> Remove(int id)
        {
            var it = await _ctx.CartItems.FindAsync(id);
            if (it == null) return NotFound();
            _ctx.CartItems.Remove(it);
            await _ctx.SaveChangesAsync();
            return Json(new { ok = true });
        }
    }
}
