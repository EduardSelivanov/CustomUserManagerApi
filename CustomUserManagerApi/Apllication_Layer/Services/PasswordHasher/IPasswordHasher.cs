namespace CustomUserManagerApi.Apllication_Layer.Services.PasswordHasher
{
    public interface IPasswordHasher
    {
        string GenerateHashedPassword(string password);
    }
}
