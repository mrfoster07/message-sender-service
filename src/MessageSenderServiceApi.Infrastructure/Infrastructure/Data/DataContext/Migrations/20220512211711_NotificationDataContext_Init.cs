using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MessageSenderServiceApi.Infrastructure.Infrastructure.Data.DataContext.Migrations
{
    public partial class NotificationDataContext_Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    JsonHash = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Json = table.Column<string>(type: "text", nullable: false),
                    IsDelivered = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_Id_IsDelivered",
                table: "Notifications",
                columns: new[] { "Id", "IsDelivered" });

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_JsonHash",
                table: "Notifications",
                column: "JsonHash");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Notifications");
        }
    }
}
