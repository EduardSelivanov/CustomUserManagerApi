namespace CustomUserManagerApi.Apllication_Layer.Services.AuthService
{
    public interface IAuthDataService
    {
        void GenerateSessionId(string userId, string sessionId);
        bool IsSessionValid(string userId, string sessionId);
    }
}
