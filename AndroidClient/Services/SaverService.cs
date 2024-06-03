using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AndroidClient.Services
{
    public class SaverService:ISaverService
    {
        private CookieCollection? _applicationCookies;
        private string _myToken;
        private string _UserName;

        public void SetCookies(CookieCollection cookie)
        {
            _applicationCookies = cookie;
        }
        public CookieCollection? GetCookie()
        {
            return _applicationCookies; 
        }

        public void SetName(string userName)
        { 
            _UserName = userName;
        }
        public string GetName()
        {
            return _UserName;
        }

        public string GetMyToken()
        {
            return _myToken;
        }
        public void SetMyToken(string myToken)
        {
            _myToken = myToken;
        }
    }
}
