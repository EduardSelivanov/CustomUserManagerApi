

namespace AndroidClient.Models
{
    public class MessagesFront
    {
        public Guid MessageId { get; set; }
        public Guid SenderId { get; set; }
        public string SenderName { get; set; }
        public Guid ChatId { get; set; }
        public string MessageText { get; set; }
        public DateTime SendedTime { get; set; }

        public bool IsMine { get; set; } 
        public LayoutOptions MessageAlignment { get; set; }
    }
}
