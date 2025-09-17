using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Foodly.Infrastructure.Data;     // <-- важный using

namespace Foodly.Web.Controllers;

[Authorize]
public class ExploreController : Controller
{
    public IActionResult Index() => View();
}

[Authorize]
[ApiController]
[Route("[controller]")]
public sealed class RestaurantsController : ControllerBase
{
    private readonly AppDbContext _db;   // <-- AppDbContext

    public RestaurantsController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var items = await _db.Restaurants
            .AsNoTracking()
            .OrderBy(r => r.Name)
            .Select(r => new
            {
                id = r.Id,
                name = r.Name,
                logo = r.CoverUrl,
                rating = 4.1 + (r.Id % 10) * 0.05,
                reviews = 20359 + r.Id,
                cuisine = "Burger",
                price = "$$",
                distanceKm = 9.0 + r.Id
            })
            .ToListAsync();

        return Ok(items);
    }
}
