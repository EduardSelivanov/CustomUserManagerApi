using CustomUserManagerApi.Domain_Layer.Models.Domains;
using CustomUserManagerApi.Domain_Layer.Models.DTOs;

namespace CustomUserManagerApi.Infrastructure_Layer.Repositories.UserRep
{

    // register, login
    public interface IUserRepo
    {
        Task<List<UserDomain>> ReturnAllusers(string token, string searchQuery = null);
        Task<UserDomain> ReturnUserById(Guid userId);
        Task<bool> RegisterNewUser(string pass1, string pass2, UserRegisterDTO newUser);
        Task<string> LoginUser(UserLoginDTO assumedUser);

        Task<UserDomain> ReturnUserByToken(string token);

        Task<List<RoleDomain>> AddRolesByAdmin(Guid affectedUserId, string newRoleS);
        Task<UserDomain> ChangePasswordByUser(ChangeUserDTO newData);

    }
}
