namespace CustomUserManagerApi.Domain_Layer.Models.DTOs.ChatDto
{
    public class MessageDTO
    {
        public Guid MessageId { get; set; }
        public Guid SenderId { get; set; }

        public string SenderName { get; set; }
        public Guid ChatId { get; set; }
        public string MessageText { get; set; }
        public DateTime SendedTime { get; set; }
    }
}
