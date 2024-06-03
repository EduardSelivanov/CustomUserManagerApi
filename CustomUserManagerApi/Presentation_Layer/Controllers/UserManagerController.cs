using CustomUserManagerApi.Apllication_Layer.Services.Middleware;
using CustomUserManagerApi.Domain_Layer.Models.Domains;
using CustomUserManagerApi.Domain_Layer.Models.DTOs;
using CustomUserManagerApi.Infrastructure_Layer.Repositories.UserRep;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CustomUserManagerApi.Presentation_Layer.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    public class UserManagerController : ControllerBase
    {
        private readonly IUserRepo _userRepo;

        public UserManagerController(IUserRepo userRepo)
        {
            _userRepo = userRepo;
        }
        //if logined show info
        //method to apply for another role to admin 
        //method to create roles-only by admin

        [HttpGet]
        [Route("GetUserInfo")]
        [Authorize(Roles = "user")]
        public async Task<IActionResult> GetUserInfo()
        {
            string jwtToken = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            UserDomain user = await _userRepo.ReturnUserByToken(jwtToken);

            //test if the same model of dbContext
            Guid id = user.UserId;
            user = await _userRepo.ReturnUserById(id);

            if (user != null)
            {
                return Ok(user);
            }
            else
            {
                return BadRequest("UserBad");
            }
        }

        [HttpGet]
        [Route("GetAllUsers")]
        [Authorize]
        public async Task<IActionResult> GetAllUsers(string searchQueryUser = null)
        {
            string jwtToken = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", ""); // space after Bearer

            List<UserDomain> usersList = await _userRepo.ReturnAllusers(jwtToken, searchQueryUser);

            return Ok(usersList);
        }

        [HttpPut]
        [Route("GrantRoleByAdmin")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GrantRoleByAdmin([FromBody] AddRoleToUserDTO bag)
        {
            Guid userId = bag.affectedUserid;
            string role = bag.RoleName;
            if (userId != Guid.Empty)
            {
                List<RoleDomain> rolesOfAffectedUser = await _userRepo.AddRolesByAdmin(userId, role);

                return Ok(rolesOfAffectedUser);
            }
            else
            {
                return BadRequest();
            }
        }
        [HttpPut]
        [Route("ChangePassword")]
        public async Task<IActionResult> Changepass([FromBody] ChangeUserDTO newData)
        {
            UserDomain us = await _userRepo.ChangePasswordByUser(newData);
            if (us != null)
            {
                return Ok(us);
            }
            else { return BadRequest(); }

        }
    }
}


