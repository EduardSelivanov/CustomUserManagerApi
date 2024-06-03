using CustomUserManagerApi.Apllication_Layer.Services.AuthService;
using CustomUserManagerApi.Apllication_Layer.Services.Token;
using CustomUserManagerApi.Domain_Layer.Models.DTOs;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Newtonsoft.Json;
using System.IO;

namespace CustomUserManagerApi.Apllication_Layer.Services.Middleware
{
    public class CookieValidationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IServiceScopeFactory _scopefactory;

        public CookieValidationMiddleware(RequestDelegate next,IServiceScopeFactory scopefactory)
        {
            
            _next = next;
            _scopefactory = scopefactory;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            using (var scope=_scopefactory.CreateScope()) {
                ITokenService itokser=scope.ServiceProvider.GetService<ITokenService>();
                IAuthDataService idataser=scope.ServiceProvider.GetService<IAuthDataService>();

                string token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                DataFromTokenDTO userfromtok = itokser.ValidateTokenAndGetAllUserData(token);

                if (!context.Request.Cookies.TryGetValue("SessionData", out string? sessionDataJson))
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("no cookies");
                    return;
                }
                UserCookiesDTO userCookies = JsonConvert.DeserializeObject<UserCookiesDTO>(sessionDataJson);
                bool f = idataser.IsSessionValid(userfromtok.UserId.ToString(), userfromtok.UserSessionId.ToString());

                if (userCookies == null || !f)
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("error with cookies");
                    return;
                }
                await _next(context);
            }
        }
    }
}
