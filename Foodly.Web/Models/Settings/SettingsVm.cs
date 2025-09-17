using Foodly.Domain.Entities;
using System.Collections.Generic;

namespace Foodly.Web.Models
{
    public class SettingsVm
    {
        public List<SubscriptionPlan> Plans { get; set; } = new();
        public UserSubscription? ActiveSubscription { get; set; }
    }
}