namespace CustomUserManagerApi.Domain_Layer.Models.DTOs
{
    public class DataFromTokenDTO
    {
        public Guid UserId { get; set; }
        public Guid UserSessionId { get; set; }
        public List<string> UserRoles { get; set; }
    }
}
