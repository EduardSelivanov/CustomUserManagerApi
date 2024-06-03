namespace CustomUserManagerApi.Domain_Layer.Models.DTOs.ChatDto
{
    public class SendMessInChatDTO
    {
        public Guid ChatId { get; set; }
        public string TextToSend { get; set; }
    }
}
