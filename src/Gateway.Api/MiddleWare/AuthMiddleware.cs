using Gateway.Application.Services;
using System.Net;
using Gateway.Api.Constants;

namespace Gateway.Api.Middleware;
public class AuthMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, IAuthValidationService authValidationService)
    {
        if (context.Request.Path.StartsWithSegments("/skills")) {
            if (!context.Request.Cookies.TryGetValue(ApiConstants.JwtToken, out var cookieValue)) {
                context.Response.StatusCode = (int)HttpStatusCode.SeeOther;
                context.Response.Headers.Location = "/auth/login";
                return;
            }

            var userInfo = await authValidationService.ValidateCookieAsync(cookieValue);

            context.Request.Headers[ApiConstants.UserIdHeader] = userInfo?.Id;
            context.Request.Headers[ApiConstants.UserNameHeader] = userInfo?.Name;
        }

        await next(context);
    }
}
