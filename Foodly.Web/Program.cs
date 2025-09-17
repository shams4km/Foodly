using Foodly.Infrastructure.Data;
using Foodly.Infrastructure.Services;
using Foodly.Web.Hubs;
using Foodly.Web.Middleware;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// EF Core + SQLServer
builder.Services.AddDbContext<AppDbContext>(o =>
    o.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Identity с типом IdentityUser
builder.Services.AddIdentity<IdentityUser, IdentityRole>(o =>
{
    o.Password.RequiredLength = 6;
    o.Password.RequireNonAlphanumeric = false;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders()
.AddDefaultUI();

// Localization
builder.Services.AddLocalization(opt => opt.ResourcesPath = "Resources");
builder.Services.AddControllersWithViews()
    .AddViewLocalization()
    .AddDataAnnotationsLocalization();

// Policies
builder.Services.AddAuthorization(opt =>
{
    opt.AddPolicy("AdminOnly", p => p.RequireRole("Admin"));
    opt.AddPolicy("ActiveSubscription", p => p.RequireClaim("HasActiveSubscription", "true"));
});

// SignalR
builder.Services.AddSignalR();

// Compression
builder.Services.AddResponseCompression();

// Mongo logger
//builder.Services.AddSingleton<MongoLogService>();

var app = builder.Build();

// Localization middleware
var cultures = new[] { "en", "ru" };
var ci = cultures.Select(c => new CultureInfo(c)).ToList();
app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("en"),
    SupportedCultures = ci,
    SupportedUICultures = ci
});

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error/ServerError");
}
app.UseStatusCodePagesWithReExecute("/Error/StatusCode", "?code={0}");

app.UseResponseCompression();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// Custom request logging -> Mongo
//app.UseMiddleware<RequestLogMiddleware>();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();
app.MapHub<ChatHub>("/hubs/chat");

// Run migrations & seed
using (var scope = app.Services.CreateScope())
{
    await Seed.RunAsync(scope.ServiceProvider);
}

app.Run();
