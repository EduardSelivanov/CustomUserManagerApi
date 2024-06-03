namespace CustomUserManagerApi.Domain_Layer.Models.Domains
{
    public class RoleDomain
    {
        public Guid UserRoleId { get; set; }

        public string RoleName { get; set; }


        public List<UserDomain> UsersWithThisRole = new List<UserDomain>();
    }
}
