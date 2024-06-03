using CustomUserManagerApi.Apllication_Layer.Services.AuthService;
using CustomUserManagerApi.Apllication_Layer.Services.PasswordHasher;
using CustomUserManagerApi.Apllication_Layer.Services.Token;
using CustomUserManagerApi.Domain_Layer.Models.Domains;
using CustomUserManagerApi.Domain_Layer.Models.DTOs;
using CustomUserManagerApi.Infrastructure_Layer.DataManager;
using Microsoft.EntityFrameworkCore;

namespace CustomUserManagerApi.Infrastructure_Layer.Repositories.UserRep
{
    public class UserRepo : IUserRepo
    {
        private readonly UserDbContext _userDbContext;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ITokenService _tok;
        private readonly IAuthDataService _authDataService;
       
        public UserRepo(UserDbContext userContext, IPasswordHasher passwordHasher, ITokenService itok, IAuthDataService authDataService)
        {
            _userDbContext = userContext;
            _passwordHasher = passwordHasher;
            _tok = itok;
            _authDataService = authDataService;
        }
        //START AUTH
        public async Task<string> LoginUser(UserLoginDTO assumedUser)
        {

            string pass = _passwordHasher.GenerateHashedPassword(assumedUser.Password);
            //err
            UserDomain? guessdUser = await _userDbContext.UsersTable.Include(u => u.RolesOfUser)
                .FirstOrDefaultAsync(u => u.UserEmail.Equals(assumedUser.UserEmail) &&
            pass.Equals(u.HashedPassword)
            );

            if (guessdUser != null)
            {
                string token = null;
                Guid sessionid = Guid.NewGuid();
                token = _tok.GenereteTokenOnLogin(guessdUser, sessionid.ToString());
                _authDataService.GenerateSessionId(guessdUser.UserId.ToString(), sessionid.ToString());

                return token;
            }
            else
            {
                return null;
            }


        }
        public async Task<bool> RegisterNewUser(string pass1, string pass2, UserRegisterDTO newUser)
        {
            string passtoHash = null;

            //pass ok?
            if (pass1.Equals(pass2))
            {
                passtoHash = pass1;
            }
            else
            {
                return false;
            }
            // sure dsnt exst
            UserDomain? asumedUser = await _userDbContext.UsersTable.FirstOrDefaultAsync(u => u.UserName.Equals(newUser.UserName));
            if (asumedUser == null)
            {
                string hashedPass = _passwordHasher.GenerateHashedPassword(passtoHash);
                RoleDomain? userRole = await _userDbContext.RolesTable.FirstOrDefaultAsync(r => r.RoleName.Equals("user"));

                asumedUser = new UserDomain()
                {
                    UserName = newUser.UserName,
                    UserEmail = newUser.UserName,
                    HashedPassword = hashedPass,

                };
                asumedUser.RolesOfUser.Add(userRole);

                // check if listofusers in role is null
                if (userRole.UsersWithThisRole == null)
                {
                    userRole.UsersWithThisRole = new List<UserDomain>();
                    userRole.UsersWithThisRole.Add(asumedUser);
                }
                else
                {
                    userRole.UsersWithThisRole.Add(asumedUser);
                }
                _userDbContext.UsersTable.Add(asumedUser);
                _userDbContext.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
        }
        //END AUTH

        // DISPLAY METHODS
        public async Task<UserDomain> ReturnUserByToken(string token)
        {
            DataFromTokenDTO userData = _tok.ValidateTokenAndGetAllUserData(token);
            
                if (userData.UserId != Guid.Empty)
                {
                    UserDomain? receivedUser = await _userDbContext.UsersTable.Include(ur => ur.RolesOfUser)
                        .Include(ud => ud.ChatsOfUser)
                        .FirstOrDefaultAsync(u => u.UserId == userData.UserId);
                    if (receivedUser != null)
                    {
                        return receivedUser;
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            
        }

        public async Task<UserDomain> ReturnUserById(Guid userId)
        {

            if (userId.Equals(Guid.Empty))
            {
                return null;
            }
            UserDomain? user = _userDbContext.UsersTable
                .Include(ud => ud.RolesOfUser).Include(ud => ud.ChatsOfUser).FirstOrDefault(ud => ud.UserId.Equals(userId));
            if (user != null)
            {
                return user;
            }
            else
            {
                return null;
            }


        }


        public async Task<List<UserDomain>> ReturnAllusers(string token, string searchQuery = null)
        {


            UserDomain me = await ReturnUserByToken(token);
            List<UserDomain> allusers = null;
            if (!string.IsNullOrEmpty(searchQuery))
            {
                allusers = _userDbContext.UsersTable.Where(ud => ud.UserName.Contains(searchQuery)).ToList();
                if (me == null || allusers.Count == 0)
                {
                    return null;
                }
            }
            else
            {
                allusers = _userDbContext.UsersTable.ToList();
                if (me == null || allusers.Count == 0)
                {
                    return null;
                }
            }
            allusers.Remove(me);
            return allusers;
        }

        // END

        // EDITING USER
        public async Task<List<RoleDomain>> AddRolesByAdmin(Guid affectedUserId, string newRoleS)
        {
            UserDomain? affectedUser = await _userDbContext.UsersTable.Include(u => u.RolesOfUser)
                .FirstOrDefaultAsync(u => u.UserId.Equals(affectedUserId));

            if (affectedUser.RolesOfUser.Any(r => r.RoleName == newRoleS))
            {
                return null;
            }
            else
            {
                //chk if role exsts
                RoleDomain? roleFromString = await _userDbContext.RolesTable.FirstOrDefaultAsync(r => r.RoleName.Equals(newRoleS));
                if (roleFromString != null)
                {
                    affectedUser.RolesOfUser.Add(roleFromString);
                    _userDbContext.SaveChanges();
                    return affectedUser.RolesOfUser;

                    // add for this user 99F3905B-33A6-4179-21B0-08DC32DDE0CC
                    // Admin role
                }
                return null;
            }
        }
        public async Task<UserDomain> ChangePasswordByUser(ChangeUserDTO newdata)
        {
            string token = newdata.JwtTokenOfUser.ToString();
            DataFromTokenDTO userData = _tok.ValidateTokenAndGetAllUserData(token);
           
                string origHashPass = _passwordHasher.GenerateHashedPassword(newdata.OriginalPass);
                string newHasPass = _passwordHasher.GenerateHashedPassword(newdata.NewPassword);
                UserDomain? affected = await _userDbContext.UsersTable.Include(u => u.RolesOfUser)
               .FirstOrDefaultAsync(u => u.UserId.Equals(userData.UserId) &&
           origHashPass.Equals(u.HashedPassword)
           );
                if (affected != null)
                {
                    affected.HashedPassword = newHasPass;
                    await _userDbContext.SaveChangesAsync();
                    return affected;
                }
                else
                {
                    return null;
                }
          


        }


        //END

    }
}
