namespace CustomUserManagerApi.Domain_Layer.Models.DTOs
{
    public class AddRoleToUserDTO
    {
        public string RoleName { get; set; }
        public Guid affectedUserid { get; set; }
    }
}
