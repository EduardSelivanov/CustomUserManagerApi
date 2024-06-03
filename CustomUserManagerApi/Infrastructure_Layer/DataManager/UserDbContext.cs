using CustomUserManagerApi.Domain_Layer.Models.Domains;
using Microsoft.EntityFrameworkCore;

namespace CustomUserManagerApi.Infrastructure_Layer.DataManager
{
    public class UserDbContext : DbContext
    {
        public UserDbContext()
        {
            //testing 
        }
        public UserDbContext(DbContextOptions<UserDbContext> opt) : base(opt) { }

        public DbSet<RoleDomain> RolesTable { get; set; }
        public DbSet<UserDomain> UsersTable { get; set; }

        public DbSet<ChatDomain> ChatsTable { get; set; }
        public DbSet<MessageDomain> MessagesTable { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RoleDomain>().HasKey(ur => ur.UserRoleId);
            modelBuilder.Entity<UserDomain>().HasKey(ud => ud.UserId);

            modelBuilder.Entity<ChatDomain>().HasKey(cht => cht.ChatId);
            modelBuilder.Entity<MessageDomain>().HasKey(md => md.MessageId);


            modelBuilder.Entity<RoleDomain>().HasData(
                new RoleDomain()
                {
                    UserRoleId = new Guid("e6b1605b-7106-4fde-9f33-6b6da9de7043"),
                    RoleName = "user"

                },
                new RoleDomain()
                {
                    UserRoleId = new Guid("40dce753-a52e-4b2c-a833-58d20e4d1927"),
                    RoleName = "Admin"

                }
            );
            modelBuilder.Entity<UserDomain>().HasData(
                new UserDomain()
                {
                    UserId = new Guid("aac3b39f-30a2-4e36-8860-a9d61c66f2a5"),
                    UserName = "admin",
                    UserEmail = "admin",
                    HashedPassword = "pmWkWSBCL51Bfkhn79xPuKBKHz//H6B+mY6G9/eieuM="
                });

            modelBuilder.Entity<RoleDomain>().HasMany(ur => ur.UsersWithThisRole).WithMany(uwr => uwr.RolesOfUser);

            modelBuilder.Entity<UserDomain>().HasMany(ud => ud.RolesOfUser).WithMany(rof => rof.UsersWithThisRole)
                .UsingEntity(ntb => ntb.HasData(
                    new
                    {// user role to adm
                        RolesOfUserUserRoleId = new Guid("e6b1605b-7106-4fde-9f33-6b6da9de7043"),
                        UsersWithThisRoleUserId = new Guid("aac3b39f-30a2-4e36-8860-a9d61c66f2a5")
                    },
                    new
                    {// adm tole to adm
                        RolesOfUserUserRoleId = new Guid("40dce753-a52e-4b2c-a833-58d20e4d1927"),//role id
                        UsersWithThisRoleUserId = new Guid("aac3b39f-30a2-4e36-8860-a9d61c66f2a5") // to user
                    }
                    ));

            modelBuilder.Entity<UserDomain>().HasMany(ud => ud.ChatsOfUser).WithMany(cou => cou.ChatMembers);
            modelBuilder.Entity<MessageDomain>().HasOne(md => md.ChatDom).WithMany(cd => cd.ChatMessages).HasForeignKey(md => md.ChatId);

            base.OnModelCreating(modelBuilder);
        }

    }
}
