using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Foodly.Infrastructure.Data;

namespace Foodly.Web.Controllers;

public class HomeController : Controller
{
    private readonly AppDbContext _db;

    public HomeController(AppDbContext db) => _db = db;

    // Страница
    [HttpGet("/")]
    [HttpGet("/home")]
    public IActionResult Index() => View();

    // === API для AJAX ===

    // Категории (в верхней плашке Explore categories)
    [HttpGet("/api/home/categories")]
    public async Task<IActionResult> Categories()
    {
        var items = await _db.Categories
            .OrderBy(c => c.Name)
            .Take(6)
            .Select(c => new {
                c.Id, c.Name, c.Slug
            })
            .ToListAsync();

        // Верну ещё и «иконку» (имя svg) по позиции
        var icons = new[] { "flame", "flash", "vip", "dine", "pickup", "map" };
        var withIcons = items.Select((c, i) => new {
            c.Id, c.Name, c.Slug, icon = icons[Math.Min(i, icons.Length-1)]
        });

        return Ok(withIcons);
    }

    // Избранные рестораны (2 ряда по 3 штуки)
    [HttpGet("/api/home/featured-restaurants")]
    public async Task<IActionResult> Featured()
    {
        var items = await _db.Restaurants
            .OrderBy(r => r.Name)
            .Take(6)
            .Select(r => new {
                r.Id, r.Name,
                logo = r.CoverUrl,         // используем Cover как «лого»
                rating = 4.0 + (r.Id % 10) / 10.0, // немножко «псевдо-рейтинга»
                km = 0.3 + (r.Id % 20) * 0.5
            })
            .ToListAsync();

        return Ok(items);
    }

    // Коллекция «Asian food» — три карточки из Product + Restaurant
    [HttpGet("/api/home/collection")]
    public async Task<IActionResult> Collection()
    {
        var items = await _db.Products
            .OrderBy(p => p.Name)
            .Take(3)
            .Select(p => new {
                p.Id, p.Name, p.Price,
                restaurant = p.Restaurant != null ? p.Restaurant.Name : "",
                // картинку можно потом хранить в Product.CoverUrl; пока подставим плейсхолдеры на фронте
            })
            .ToListAsync();

        return Ok(items);
    }
}
