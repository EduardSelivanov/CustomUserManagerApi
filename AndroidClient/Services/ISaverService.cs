using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AndroidClient.Services
{
    public interface ISaverService
    {
        void SetCookies(CookieCollection cookie);
        CookieCollection? GetCookie();

        string GetMyToken();
        void SetMyToken(string token);

        public void SetName(string userName);
        string GetName();
    }
}
