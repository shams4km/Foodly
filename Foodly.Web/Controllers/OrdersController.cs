using System;
using System.Linq;
using System.Threading.Tasks;
using Foodly.Infrastructure.Data;
using Foodly.Web.Models.Orders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Foodly.Web.Controllers
{
    [Authorize]
    [Route("orders")]
    public class OrdersController : Controller
    {
        private readonly AppDbContext _db;
        private readonly UserManager<IdentityUser> _users;

        public OrdersController(AppDbContext db, UserManager<IdentityUser> users)
        {
            _db = db;
            _users = users;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var uid = _users.GetUserId(User);

            // Заказы пользователя + позиции
            var orders = await _db.Orders
                .Where(o => uid != null && o.UserId == uid)
                .OrderByDescending(o => o.CreatedAt)
                .Include(o => o.Items)
                .ToListAsync();

            // Все productId из заказов
            var productIds = orders.SelectMany(o => o.Items)
                                   .Select(i => i.ProductId)
                                   .Distinct()
                                   .ToList();

            // Подтянем продукты (id -> {name, restaurantId})
            var products = await _db.Products
                .Where(p => productIds.Contains(p.Id))
                .Select(p => new { p.Id, p.Name, p.RestaurantId })
                .ToListAsync();

            var productNameById = products.ToDictionary(p => p.Id, p => p.Name);
            var productRestById  = products.ToDictionary(p => p.Id, p => p.RestaurantId);

            // Соберём restaurantId из продуктов и подтянем названия ресторанов
            var restaurantIds = products.Select(p => p.RestaurantId).Distinct().ToList();
            var restaurantNameById = await _db.Restaurants
                .Where(r => restaurantIds.Contains(r.Id))
                .ToDictionaryAsync(r => r.Id, r => r.Name);

            var vm = new OrdersVm();

            foreach (var o in orders)
            {
                // Определим ресторан по первому item'у заказа
                string restaurantTitle = "Restaurant";
                var firstProductId = o.Items.OrderBy(i => i.Id).Select(i => i.ProductId).FirstOrDefault();
                if (firstProductId != 0 && productRestById.TryGetValue(firstProductId, out var rid))
                {
                    if (restaurantNameById.TryGetValue(rid, out var rname))
                        restaurantTitle = rname;
                }

                var card = new OrderCardVm
                {
                    Id = o.Id,
                    OrderNumber = $"#{o.Id:X5}",
                    Restaurant = restaurantTitle,
                    CreatedAt = o.CreatedAt,
                    Status = o.Status
                };

                var lines = o.Items
                    .OrderBy(i => i.Id)
                    .Select(i => new OrderLineVm
                    {
                        Qty = i.Quantity,
                        Title = productNameById.TryGetValue(i.ProductId, out var pname) ? pname : "Item"
                    })
                    .ToList();

                card.HiddenItemsCount = Math.Max(0, lines.Count - 2);
                card.Lines = lines.Take(2).ToList();

                // Активные в "Upcoming", завершённые/отменённые — в "Previous"
                var completed = string.Equals(o.Status, "Completed", StringComparison.OrdinalIgnoreCase);
                var canceled  = string.Equals(o.Status, "Canceled",  StringComparison.OrdinalIgnoreCase);

                if (!completed && !canceled)
                {
                    var mins = 30 + (o.Id % 40);
                    card.ETA = $"{mins} min";
                    vm.Upcoming.Add(card);
                }
                else
                {
                    vm.Previous.Add(card);
                }
            }

            // Демо-карточки, если нет активных — для визуального заполнения страницы
            if (!vm.Upcoming.Any())
            {
                vm.Upcoming.AddRange(new[]
                {
                    new OrderCardVm{
                        Id=1, OrderNumber="#1DF90E", Restaurant="Burger King", ETA="35 min", Status="Processing",
                        Lines = new(){ new(){Qty=1, Title="Gigantic Rodeo Burger"}, new(){Qty=1, Title="Fries L"}}
                    },
                    new OrderCardVm{
                        Id=2, OrderNumber="#3E0A49", Restaurant="McDonald's", ETA="60 min", Status="OnTheWay",
                        Lines = new(){ new(){Qty=1, Title="Medium Fries Deluxe"}, new(){Qty=2, Title="Chicken Nuggets"}}
                    }
                });
            }

            return View(vm);
        }
    }
}
