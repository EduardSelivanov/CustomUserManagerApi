using CustomUserManagerApi.Apllication_Layer.Services.Middleware;
using CustomUserManagerApi.Apllication_Layer.Services.SignalR;
using CustomUserManagerApi.Domain_Layer.Models.Domains;
using CustomUserManagerApi.Domain_Layer.Models.DTOs.ChatDto;
using CustomUserManagerApi.Infrastructure_Layer.Repositories.ChatRep;
using CustomUserManagerApi.Infrastructure_Layer.Repositories.UserRep;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace CustomUserManagerApi.Presentation_Layer.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly IUserRepo _userRepo;
        private readonly IChatRepo _chatRepo;
        private readonly IHubContext<MyChatHub> _chatHubContext;

        public ChatController(IUserRepo userRepo, IChatRepo chatRepo)
        {
            _userRepo = userRepo;
            _chatRepo = chatRepo;
        }

        [HttpPost]
        [Route("createChat")]
        [Authorize]
        public async Task<IActionResult> CreateChat([FromBody] Guid opponentId)
        {
            string myToken = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            UserDomain me = await _userRepo.ReturnUserByToken(myToken);
            UserDomain opponent = await _userRepo.ReturnUserById(opponentId);

            if (me == null || opponent == null)
            {
                return BadRequest();
            }
            if (me.UserId.Equals(opponentId))
            {
                return BadRequest();
            }
            List<UserDomain> users = [me, opponent];
            Guid chatId = await _chatRepo.CreateChat(users);
            if (chatId.Equals(Guid.Empty))
            {
                return BadRequest();
            }
            return Ok(chatId);
        }



        [HttpPost]
        [Route("sendMess")]
        [Authorize]
        public async Task<IActionResult> SendMessageInChat([FromBody] SendMessInChatDTO mess)
        {
            string myToken = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            UserDomain me = await _userRepo.ReturnUserByToken(myToken);

            if (me == null || mess.ChatId.Equals(Guid.Empty))
            {
                return BadRequest();
            }
            bool succes = await _chatRepo.SendMessage(mess.ChatId, me, mess.TextToSend);
            if (succes)
            {

                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpGet]
        [Route("ShowCertainChat/{ChatId}/{page}")]
        [Authorize]
        public async Task<IActionResult> ShowCertainChat(Guid chatId, int page)
        {
            string myToken = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            UserDomain me = await _userRepo.ReturnUserByToken(myToken);

            List<MessageDTO> mess = await _chatRepo.ShowMessagesInChat(chatId, page, me);
            if (mess == null)
            {
                return NotFound();
            }

            return Ok(mess);
        }


        [HttpGet]
        [Route("ShowMyChats")]
        [Authorize]
        public async Task<IActionResult> ShowChatsForMe()
        {
            string jwtTOken = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            UserDomain me = await _userRepo.ReturnUserByToken(jwtTOken);
            List<ShowChatDTO> myChats = await _chatRepo.ShowMyChats(me);
            if (myChats.Count == 0)
            {
                return BadRequest();
            }

            return Ok(myChats);
        }

    }
}
