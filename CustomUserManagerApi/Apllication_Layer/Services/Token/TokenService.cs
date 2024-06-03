using CustomUserManagerApi.Domain_Layer.Models.Domains;
using CustomUserManagerApi.Domain_Layer.Models.DTOs;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CustomUserManagerApi.Apllication_Layer.Services.Token
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _icon;
        public TokenService(IConfiguration icon)
        {
            _icon = icon;
        }
        TokenValidator _tokval = new TokenValidator();

        public string GenereteTokenOnLogin(UserDomain userLogin, string sessionId)
        {
            List<Claim> cls = new List<Claim>();
            cls.Add(new Claim(ClaimTypes.Email, userLogin.UserEmail));
            cls.Add(new Claim(ClaimTypes.NameIdentifier, userLogin.UserId.ToString()));
            cls.Add(new Claim("SessionId", sessionId));

            foreach (RoleDomain role in userLogin.RolesOfUser)
            {
                cls.Add(new Claim(ClaimTypes.Role, role.RoleName));
            }

            var roles = cls.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToList();

            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_icon["Jwt:Key"]));

            SigningCredentials creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            JwtSecurityToken token = new JwtSecurityToken(
                issuer: _icon.GetSection("Jwt:Issuer").Value,
                audience: _icon.GetSection("Jwt:Audience").Value,
                claims: cls,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds
                );
            string tokenstr = new JwtSecurityTokenHandler().WriteToken(token);

            return tokenstr;
        }

        public DataFromTokenDTO ValidateTokenAndGetAllUserData(string token)
        {
            TokenValidationParameters vlidationParameters = _tokval.ValidationOfToken(_icon);
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                SecurityToken asumedToken = null;
                ClaimsPrincipal claims = tokenHandler.ValidateToken(token, vlidationParameters, out asumedToken);

                DataFromTokenDTO usersData = new DataFromTokenDTO
                {
                    UserId = GetUserIdFromClaims(claims),
                    UserSessionId = GetSessionFromClaims(claims),
                    UserRoles = GetUserRolesFromClaims(claims)
                };
                return usersData;
            }
            catch (Exception ex)
            {
                throw new SecurityTokenValidationException("Token ERROR.", ex);
                //return Guid.Empty;
            }
        }
        private Guid GetUserIdFromClaims(ClaimsPrincipal claimsToParse)
        {
            if (claimsToParse == null)
            {
                return Guid.Empty;
            }
            else
            {
                Claim? claimuserId = claimsToParse.FindFirst(ClaimTypes.NameIdentifier);
                Guid userId = Guid.Parse(claimuserId.Value);
                List<Claim> claimsRoles = claimsToParse.Claims.Where(c => c.Type == ClaimTypes.Role).ToList().ToList();
                List<string> roles = claimsRoles.Select(cr => cr.Value).ToList();
                return userId;
            }
        }
        private Guid GetSessionFromClaims(ClaimsPrincipal calimsToParse)
        {
            if (calimsToParse == null)
            {
                throw new Exception();
            }
            else
            {
                Claim sessionId = calimsToParse.FindFirst("SessionId");
                string sesstr = sessionId.Value;
                Guid sessionIdStr = Guid.Parse(sesstr);
                return sessionIdStr;
            }

        }
        private List<string> GetUserRolesFromClaims(ClaimsPrincipal claimsToParse)
        {
            if (claimsToParse == null)
            {
                return null;
            }
            else
            {
                Claim? claimuserId = claimsToParse.FindFirst(ClaimTypes.NameIdentifier);
                Guid userId = Guid.Parse(claimuserId.Value);
                List<Claim> claimsRoles = claimsToParse.Claims.Where(c => c.Type == ClaimTypes.Role).ToList().ToList();
                List<string> roles = claimsRoles.Select(cr => cr.Value).ToList();
                return roles;
            }

        }
    }
}
