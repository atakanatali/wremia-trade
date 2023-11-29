namespace WremiaTrade.Common.Middlewares
{
    using Microsoft.AspNetCore.Http;

    using System.Threading.Tasks;
    
    /// <summary>
    /// This middleware support for basic bearer authentication
    /// It Checks access token and authorization header value
    /// </summary>
    public class BearerAuthenticationMiddleware
    {
        private readonly RequestDelegate _next;

        /// <summary>
        /// Ctor of middleware
        /// </summary>
        /// <param name="next"></param>
        public BearerAuthenticationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        /// <summary>
        /// Controls the access token header value if exists in header
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task InvokeAsync(HttpContext context)
        {
            string accessToken = context.Request.Cookies["access_token"];

            if (accessToken != null && string.IsNullOrEmpty(context.Request.Headers.Keys.First(p => p.Equals("Authorization"))))
            {
                context.Request.Headers.Append("Authorization", "Bearer " + accessToken);
                context.Request.Headers["Authorization"] = "Bearer " + accessToken;
            }

            await _next(context);
        }
    }
}
