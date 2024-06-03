using Microsoft.AspNetCore.Identity.UI.Services;

namespace CustomUserManagerApi.Domain_Layer.Models.Domains
{
    public class MessageDomain
    {
        public Guid MessageId { get; set; }

        public UserDomain? Sender { get; set; }

        public Guid SenderId { get; set; }

        public Guid ChatId { get; set; }
        public ChatDomain ChatDom { get; set; }
        public string MessageText { get; set; }
        public DateTime SendedTime { get; set; }
    }
}