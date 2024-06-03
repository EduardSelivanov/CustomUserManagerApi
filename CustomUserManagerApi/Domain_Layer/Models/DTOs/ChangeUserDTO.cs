namespace CustomUserManagerApi.Domain_Layer.Models.DTOs
{
    public class ChangeUserDTO
    {
        public string JwtTokenOfUser { get; set; }
        public string OriginalPass { get; set; }
        public string NewPassword { get; set; }
    }
}
