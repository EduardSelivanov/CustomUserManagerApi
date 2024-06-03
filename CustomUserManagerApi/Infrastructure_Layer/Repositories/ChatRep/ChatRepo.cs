using CustomUserManagerApi.Domain_Layer.Models.Domains;
using CustomUserManagerApi.Domain_Layer.Models.DTOs.ChatDto;
using CustomUserManagerApi.Infrastructure_Layer.DataManager;
using Microsoft.EntityFrameworkCore;


namespace CustomUserManagerApi.Infrastructure_Layer.Repositories.ChatRep
{
    public class ChatRepo : IChatRepo
    {
        private readonly UserDbContext _context;
        public ChatRepo(UserDbContext context)
        {
            _context = context;
        }
        public async Task<Guid> CreateChat(List<UserDomain> users)
        {
            List<ChatDomain> firstUser = users[0].ChatsOfUser.ToList();
            List<ChatDomain> secondUser = users[1].ChatsOfUser.ToList();
            var intersect = firstUser.Intersect(secondUser).FirstOrDefault();
            if (intersect != null)
            {
                // return here an intersected ChatGuid
                return intersect.ChatId;
            }

            ChatDomain chat = new ChatDomain();
            for (int i = 0; i < users.Count; i++)
            {
                if (users[i] == null)
                {
                    return Guid.Empty;
                }
                chat.ChatMembers.Add(users[i]);
            }
            await _context.ChatsTable.AddAsync(chat);
            await _context.SaveChangesAsync();
            return chat.ChatId;
        }

        public async Task<bool> SendMessage(Guid chatId, UserDomain sender, string messageText)
        {
            ChatDomain? cht = await _context.ChatsTable.FirstOrDefaultAsync(ch => ch.ChatId.Equals(chatId)); ;
            if (cht == null || sender == null)
            {
                return false;
            }

            MessageDomain message = new MessageDomain()
            {
                Sender = sender,
                SenderId = sender.UserId,
                ChatId = chatId,
                ChatDom = cht,
                MessageText = messageText,
                SendedTime = DateTime.UtcNow
            };

            cht.ChatMessages.Add(message);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<List<ShowChatDTO>> ShowMyChats(UserDomain me)
        {
            List<ChatDomain> myChats = me.ChatsOfUser.ToList();

            List<ShowChatDTO> chats = new List<ShowChatDTO>();
            foreach (ChatDomain chat in myChats)
            {
                ChatDomain chaaat = await _context.ChatsTable.Include(cd => cd.ChatMembers).FirstOrDefaultAsync(cd => cd.ChatId == chat.ChatId);
                List<UserDomain> users = chaaat.ChatMembers;
                foreach (UserDomain user in users)
                {
                    if (user.UserId == me.UserId)
                    {
                        continue;
                    }
                    chats.Add(new ShowChatDTO()
                    {
                        OpponentName = user.UserName,
                        ChatId = chat.ChatId
                    });
                }
            }
            return chats;
        }

        public async Task<List<MessageDTO>> ShowMessagesInChat(Guid chatId, int page, UserDomain currentUser)
        {
            ChatDomain currentChat =
                await _context.ChatsTable.Include(ch => ch.ChatMessages).FirstOrDefaultAsync(ch => ch.ChatId == chatId);

            int countOnPage = 15;

            int skippedRange = countOnPage * (page - 1);
            if (skippedRange < 0)
            {
                skippedRange = 0;
            }

            if (currentChat == null)
            {
                return null;
            }

            List<MessageDomain> messagesInChat =
                currentChat.ChatMessages.OrderByDescending(md => md.SendedTime).Skip(skippedRange).Take(countOnPage).ToList();

            List<MessageDTO> messagesDTO = new List<MessageDTO>();
            foreach (var message in messagesInChat)
            {
                UserDomain opponent = await _context.UsersTable.FirstOrDefaultAsync(ud => ud.UserId.Equals(message.SenderId)); //KOSTYLYLISHE PPC
                MessageDTO mess = new MessageDTO()
                {
                    MessageId = message.MessageId,
                    SenderId = message.SenderId,
                    SenderName = opponent.UserName,
                    ChatId = message.ChatId,
                    MessageText = message.MessageText,
                    SendedTime = message.SendedTime
                };
                messagesDTO.Add(mess);
            }

            return messagesDTO;
        }

    }
}
