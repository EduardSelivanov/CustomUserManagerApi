using CustomUserManagerApi.Domain_Layer.Models.DTOs;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace CustomUserManagerApi.Apllication_Layer.Services.AuthService
{
    public class AuthDataService:IAuthDataService
    {
        private readonly IHttpContextAccessor _ihttpContAcc;
        public AuthDataService(IHttpContextAccessor ihttpCont)
        {
            _ihttpContAcc = ihttpCont;
        }
        public void GenerateSessionId(string userId, string sessionId)
        {
            var options = new CookieOptions
            {
                Expires = DateTime.Now.AddMinutes(30),
                HttpOnly = false
            };
            var usag = _ihttpContAcc.HttpContext.Request.Headers["User-Agent"].ToString();
            UserCookiesDTO us = new UserCookiesDTO { UserId = userId, SessionId = sessionId};
            string serObj = JsonConvert.SerializeObject(us);
            _ihttpContAcc.HttpContext.Response.Cookies.Append("SessionData", serObj, options);
        }
        private UserCookiesDTO GetSession()
        {
            var serObj = _ihttpContAcc.HttpContext.Request.Cookies["SessionData"];
            UserCookiesDTO userFromJsn = JsonConvert.DeserializeObject<UserCookiesDTO>(serObj);
            return userFromJsn;

        }

        public  bool IsSessionValid(string userId, string sessionId)
        {
            UserCookiesDTO user = GetSession();
           // var currentAgent = _ihttpContAcc.HttpContext.Request.Headers["User-Agent"].ToString();
            if (user == null)
            {
                return false;
            }
            else
            {
                if (user.UserId == userId && user.SessionId == sessionId)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

        }
    }
}
