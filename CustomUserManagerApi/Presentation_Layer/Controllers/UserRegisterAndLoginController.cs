using CustomUserManagerApi.Domain_Layer.Models.DTOs;
using CustomUserManagerApi.Infrastructure_Layer.Repositories.UserRep;
using Microsoft.AspNetCore.Mvc;

namespace CustomUserManagerApi.Presentation_Layer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserRegisterAndLoginController : ControllerBase
    {
        private readonly IUserRepo userRepo;

        public UserRegisterAndLoginController(IUserRepo userRepo)
        {
            this.userRepo = userRepo;
        }

        //Register-add to database

        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterDTO newUser, string pass1, string pass2)
        {
            bool registerRes = await userRepo.RegisterNewUser(pass1, pass2, newUser);
            if (registerRes)
            {
                return Ok("Successful registration");
            }
            else
            {
                return BadRequest();
            }
        }

        // login-check if pass and email ok

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDTO loginedUser)
        {
            // create ssn via repo
            // add ssnID to token

            var token = await userRepo.LoginUser(loginedUser);
            return Ok(token);
        }

    }
}
