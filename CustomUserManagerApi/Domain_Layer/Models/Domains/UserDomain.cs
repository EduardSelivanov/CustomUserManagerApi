namespace CustomUserManagerApi.Domain_Layer.Models.Domains
{
    public class UserDomain
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; }

        public string UserEmail { get; set; }

        public string HashedPassword { get; set; }

        public List<RoleDomain> RolesOfUser { get; set; } = new List<RoleDomain>();



        public List<ChatDomain> ChatsOfUser { get; set; } = new List<ChatDomain>();
    }
}
