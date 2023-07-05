using System.Globalization;
using Novels.Models;
using System.Text.Json;
using Microsoft.Extensions.Options;

namespace Novels.Middleware
{

    public class IsAdmin
    {
        private readonly RequestDelegate _next;
        private readonly JWTSettings _jwtsettings;
        public IsAdmin(RequestDelegate next, IOptions<JWTSettings> jwtsettings)
        {
            _next = next;
            _jwtsettings = jwtsettings.Value;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            var header = (string)context.Request.Headers["Authorization"];
            Console.WriteLine("Context" + " "+ header.Substring(7));
            var dbcontext = context.RequestServices.GetRequiredService<NovelStoreContext>();
            User user = JWTSettings.GetUserFromAccessToken(header.Substring(7), _jwtsettings.Secretkey, dbcontext);
            Console.WriteLine("user" + " " + user.isAdmin);
            if (user == null || user.isAdmin == false)
            {
                context.Response.StatusCode = 403;
                return;
            }

            Console.WriteLine("admin" + " " + user.FullName);
            await _next(context);
        }

    }
    public static class IsAdminMiddlewareExtensions
    {
        public static IApplicationBuilder UseIsAdmin(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<IsAdmin>();
        }
    }
}
