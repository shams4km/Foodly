using Foodly.Domain.Entities;
using Foodly.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Foodly.Web.Controllers
{
    [Authorize]
    public class SettingsController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly AppDbContext _ctx;

        public SettingsController(UserManager<IdentityUser> um, AppDbContext ctx)
        {
            _userManager = um;
            _ctx = ctx;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var plans = await _ctx.SubscriptionPlans
                .OrderBy(p => p.Level)
                .ToListAsync();

            var sub = await _ctx.UserSubscriptions
                .FirstOrDefaultAsync(s => s.UserId == user.Id && s.IsActive);

            var vm = new SettingsVm
            {
                Plans = plans,
                ActiveSubscription = sub
            };

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> ToggleSubscription(int planId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var sub = await _ctx.UserSubscriptions
                .FirstOrDefaultAsync(s => s.UserId == user.Id && s.PlanId == planId);

            if (sub == null)
            {
                // новый юзер → создаём подписку на месяц
                sub = new UserSubscription
                {
                    UserId = user.Id,
                    PlanId = planId,
                    StartDate = DateTime.UtcNow,
                    EndDate = DateTime.UtcNow.AddMonths(1),
                    IsActive = true
                };
                _ctx.UserSubscriptions.Add(sub);
            }
            else
            {
                // переключаем статус
                sub.IsActive = !sub.IsActive;
                if (sub.IsActive)
                {
                    sub.StartDate = DateTime.UtcNow;
                    sub.EndDate = DateTime.UtcNow.AddMonths(1);
                }
            }

            await _ctx.SaveChangesAsync();

            return Json(new { ok = true, active = sub.IsActive, planId });
        }
    }

    // Вспомогательная модель для View
    public class SettingsVm
    {
        public List<SubscriptionPlan> Plans { get; set; } = new();
        public UserSubscription? ActiveSubscription { get; set; }
    }
}
