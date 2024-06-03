using CustomUserManagerApi.Domain_Layer.Models.Domains;
using CustomUserManagerApi.Domain_Layer.Models.DTOs;

namespace CustomUserManagerApi.Apllication_Layer.Services.Token
{
    public interface ITokenService
    {
        //get token while loggining
        string GenereteTokenOnLogin(UserDomain userLogin, string sessionId);

        DataFromTokenDTO ValidateTokenAndGetAllUserData(string token);

    }
}
