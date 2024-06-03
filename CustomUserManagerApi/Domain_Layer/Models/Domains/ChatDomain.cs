namespace CustomUserManagerApi.Domain_Layer.Models.Domains
{
    public class ChatDomain
    {
        public Guid ChatId { get; set; }

        public List<UserDomain> ChatMembers { get; set; } = new List<UserDomain>();

        public List<MessageDomain> ChatMessages { get; set; } = new List<MessageDomain>();
    }
}
