using System;
using System.Collections.Generic;

namespace Foodly.Web.Models.Orders
{
    public class OrdersVm
    {
        public List<OrderCardVm> Upcoming { get; set; } = new();
        public List<OrderCardVm> Previous { get; set; } = new();
    }

    public class OrderCardVm
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; } = "#";
        public string Restaurant { get; set; } = "";
        public DateTime CreatedAt { get; set; }
        public string ETA { get; set; } = "";                   // e.g. "35 min"
        public string Status { get; set; } = "Processing";      // Processing | Completed | Canceled
        public List<OrderLineVm> Lines { get; set; } = new();
        public int HiddenItemsCount { get; set; }               // для «X more items»
    }

    public class OrderLineVm
    {
        public int Qty { get; set; }
        public string Title { get; set; } = "";
    }
}
