namespace Souq.Web.Middlewares
{
    public class RoleBasedRedirectMiddleware
    {
        private readonly RequestDelegate _next;

        public RoleBasedRedirectMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            //if (context.User.Identity.IsAuthenticated)
            //{
            //    if (context.User.IsInRole("Admin") && context.Request.Path == "/")
            //    {
            //        context.Response.Redirect("/Admin/Dashboard");
            //    }
            //    else if (context.User.IsInRole("Customer") && context.Request.Path == "/")
            //    {
            //        context.Response.Redirect("/Customer/Home");
            //    }
            //}

            await _next(context);
        }
    }

    // Extension method to use the middleware
    public static class RoleBasedRedirectMiddlewareExtensions
    {
        public static IApplicationBuilder UseRoleBasedRedirect(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RoleBasedRedirectMiddleware>();
        }
    }

}
