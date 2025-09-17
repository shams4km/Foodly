using Microsoft.AspNetCore.Mvc;

namespace Foodly.Web.Controllers;

public class MessagesController : Controller
{
    // Простые DTO для страницы
    public record MessageItem(
        int Id,
        string FromName,
        string FromTitle,
        string Avatar,         // относительный путь к svg/png в wwwroot
        string Preview,
        string[] Tags,
        string TimeAgo,
        bool Unread);

    public record Bubble(
        string Side,           // "left" | "right"
        string Text,
        string Time);

    public record ThreadVm(
        string UserName,
        string Email,
        string Avatar,
        string DateHeader,
        List<Bubble> Bubbles);

    public record MessagesVm(
        List<MessageItem> Items,
        ThreadVm Thread,
        string Sort = "Recent");

    [HttpGet("/messages")]
    public IActionResult Index()
    {
        // ЛЕВАЯ колонка – список сообщений (демо-данные)
        var items = new List<MessageItem>
        {
            new(1, "John Smith", "CEO", "/icons/user-1.png",
                "Special Offer Just for You! We are running a new promotion in your area…",
                new[] { "Promotion", "Expires soon" }, "2 h ago", true),

            new(2, "Anna", "Support", "/icons/user-2.png",
                "RE: Late Delivery Refund Request", Array.Empty<string>(), "Yesterday", false),

            new(3, "Mark Spencer", "Driver", "/icons/user-3.png",
                "Order is on the way 🚗", Array.Empty<string>(), "3 days ago", false),

            new(4, "Samantha", "Marketing Manager", "/icons/user-4.png",
                "Monthly Newsletter: September", new[] { "Promotion" }, "8 Sep 2020", false),

            new(5, "James", "Support", "/icons/user-5.png",
                "RE: Special Promotion Inquiry", Array.Empty<string>(), "30 Aug 2020", false)
        };

        // ПРАВАЯ колонка – активный тред
        var thread = new ThreadVm(
            "John Smith, CEO",
            "john@nibble.com",
            "/icons/user-1.png",
            "18 Sep 2020",
            new List<Bubble>
            {
                new("left",  "Hi, I didn't get my order on time today. 🥲 Could you please refund me for a delivery? Thanks.", "2:07 PM"),
                new("right", "Hello! The new dishes are available starting from today.🍜 You can check them out here.", "4:41 PM"),
                new("left",  "Hi, thanks! I will check it out as soon as I can. 🔥", "12:33 AM"),
                new("right", "Hi, we are running a new promotion in your area. If you want to get a free delivery, then use code **FREE** at the checkout. 😄", "8:29 AM"),
            }
        );

        var vm = new MessagesVm(items, thread);
        ViewData["Title"] = "Messages";
        return View(vm);
    }
}
