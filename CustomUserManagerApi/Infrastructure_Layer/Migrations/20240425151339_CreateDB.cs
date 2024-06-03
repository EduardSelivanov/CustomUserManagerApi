using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CustomUserManagerApi.Migrations
{
    /// <inheritdoc />
    public partial class CreateDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChatsTable",
                columns: table => new
                {
                    ChatId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatsTable", x => x.ChatId);
                });

            migrationBuilder.CreateTable(
                name: "RolesTable",
                columns: table => new
                {
                    UserRoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolesTable", x => x.UserRoleId);
                });

            migrationBuilder.CreateTable(
                name: "UsersTable",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HashedPassword = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsersTable", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "ChatDomainUserDomain",
                columns: table => new
                {
                    ChatMembersUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChatsOfUserChatId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatDomainUserDomain", x => new { x.ChatMembersUserId, x.ChatsOfUserChatId });
                    table.ForeignKey(
                        name: "FK_ChatDomainUserDomain_ChatsTable_ChatsOfUserChatId",
                        column: x => x.ChatsOfUserChatId,
                        principalTable: "ChatsTable",
                        principalColumn: "ChatId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChatDomainUserDomain_UsersTable_ChatMembersUserId",
                        column: x => x.ChatMembersUserId,
                        principalTable: "UsersTable",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MessagesTable",
                columns: table => new
                {
                    MessageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SenderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChatId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MessageText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SendedTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessagesTable", x => x.MessageId);
                    table.ForeignKey(
                        name: "FK_MessagesTable_ChatsTable_ChatId",
                        column: x => x.ChatId,
                        principalTable: "ChatsTable",
                        principalColumn: "ChatId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MessagesTable_UsersTable_SenderId",
                        column: x => x.SenderId,
                        principalTable: "UsersTable",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RoleDomainUserDomain",
                columns: table => new
                {
                    RolesOfUserUserRoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UsersWithThisRoleUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleDomainUserDomain", x => new { x.RolesOfUserUserRoleId, x.UsersWithThisRoleUserId });
                    table.ForeignKey(
                        name: "FK_RoleDomainUserDomain_RolesTable_RolesOfUserUserRoleId",
                        column: x => x.RolesOfUserUserRoleId,
                        principalTable: "RolesTable",
                        principalColumn: "UserRoleId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoleDomainUserDomain_UsersTable_UsersWithThisRoleUserId",
                        column: x => x.UsersWithThisRoleUserId,
                        principalTable: "UsersTable",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "RolesTable",
                columns: new[] { "UserRoleId", "RoleName" },
                values: new object[,]
                {
                    { new Guid("40dce753-a52e-4b2c-a833-58d20e4d1927"), "Admin" },
                    { new Guid("e6b1605b-7106-4fde-9f33-6b6da9de7043"), "user" }
                });

            migrationBuilder.InsertData(
                table: "UsersTable",
                columns: new[] { "UserId", "HashedPassword", "UserEmail", "UserName" },
                values: new object[] { new Guid("aac3b39f-30a2-4e36-8860-a9d61c66f2a5"), "pmWkWSBCL51Bfkhn79xPuKBKHz//H6B+mY6G9/eieuM=", "admin", "admin" });

            migrationBuilder.InsertData(
                table: "RoleDomainUserDomain",
                columns: new[] { "RolesOfUserUserRoleId", "UsersWithThisRoleUserId" },
                values: new object[,]
                {
                    { new Guid("40dce753-a52e-4b2c-a833-58d20e4d1927"), new Guid("aac3b39f-30a2-4e36-8860-a9d61c66f2a5") },
                    { new Guid("e6b1605b-7106-4fde-9f33-6b6da9de7043"), new Guid("aac3b39f-30a2-4e36-8860-a9d61c66f2a5") }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChatDomainUserDomain_ChatsOfUserChatId",
                table: "ChatDomainUserDomain",
                column: "ChatsOfUserChatId");

            migrationBuilder.CreateIndex(
                name: "IX_MessagesTable_ChatId",
                table: "MessagesTable",
                column: "ChatId");

            migrationBuilder.CreateIndex(
                name: "IX_MessagesTable_SenderId",
                table: "MessagesTable",
                column: "SenderId");

            migrationBuilder.CreateIndex(
                name: "IX_RoleDomainUserDomain_UsersWithThisRoleUserId",
                table: "RoleDomainUserDomain",
                column: "UsersWithThisRoleUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChatDomainUserDomain");

            migrationBuilder.DropTable(
                name: "MessagesTable");

            migrationBuilder.DropTable(
                name: "RoleDomainUserDomain");

            migrationBuilder.DropTable(
                name: "ChatsTable");

            migrationBuilder.DropTable(
                name: "RolesTable");

            migrationBuilder.DropTable(
                name: "UsersTable");
        }
    }
}
