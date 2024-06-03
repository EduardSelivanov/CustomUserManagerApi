using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace CustomUserManagerApi.Apllication_Layer.Services.Token
{
    public class TokenValidator
    {
        public TokenValidationParameters ValidationOfToken(IConfiguration _iconfig)
        {
            TokenValidationParameters vlidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _iconfig.GetSection("Jwt:Issuer").Value,
                ValidAudience = _iconfig.GetSection("Jwt:Audience").Value,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                    _iconfig.GetSection("jwt:Key").Value)) //using SystemText
            };
            return vlidationParameters;
        }
    }
}
