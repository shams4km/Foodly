using System.Diagnostics;
using Foodly.Infrastructure.Services;

namespace Foodly.Web.Middleware
{
    public class RequestLogMiddleware
    {
        private readonly RequestDelegate _next;
        public RequestLogMiddleware(RequestDelegate next) => _next = next;

        public async Task Invoke(HttpContext ctx, MongoLogService mongo)
        {
            var sw = Stopwatch.StartNew();
            await _next(ctx);
            sw.Stop();

            var userId = ctx.User?.Identity?.IsAuthenticated == true
                ? (ctx.User.Identity?.Name ?? "")
                : null;

            await mongo.WriteAsync(userId, ctx.Request.Path, ctx.Request.Method, ctx.Response.StatusCode, sw.ElapsedMilliseconds);
        }
    }
}
