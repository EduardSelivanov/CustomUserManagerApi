using CustomUserManagerApi.Domain_Layer.Models.Domains;
using CustomUserManagerApi.Domain_Layer.Models.DTOs.ChatDto;


namespace CustomUserManagerApi.Infrastructure_Layer.Repositories.ChatRep
{
    public interface IChatRepo
    {

        Task<Guid> CreateChat(List<UserDomain> users);
        Task<bool> SendMessage(Guid chatId, UserDomain sender, string messageText);
        Task<List<ShowChatDTO>> ShowMyChats(UserDomain me);
        Task<List<MessageDTO>> ShowMessagesInChat(Guid chatId, int page, UserDomain currentUser);
    }
}
