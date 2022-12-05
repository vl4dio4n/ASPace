using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASPace.Data.Migrations
{
    public partial class AlteredGroupModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Groups_RegisteredUsers_RegisteredUserId",
                table: "Groups");

            migrationBuilder.DropIndex(
                name: "IX_Groups_RegisteredUserId",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "RegisteredUserId",
                table: "Groups");

            migrationBuilder.AlterColumn<string>(
                name: "Username",
                table: "RegisteredUsers",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Groups_CreatorId",
                table: "Groups",
                column: "CreatorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Groups_RegisteredUsers_CreatorId",
                table: "Groups",
                column: "CreatorId",
                principalTable: "RegisteredUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Groups_RegisteredUsers_CreatorId",
                table: "Groups");

            migrationBuilder.DropIndex(
                name: "IX_Groups_CreatorId",
                table: "Groups");

            migrationBuilder.AlterColumn<string>(
                name: "Username",
                table: "RegisteredUsers",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AddColumn<int>(
                name: "RegisteredUserId",
                table: "Groups",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Groups_RegisteredUserId",
                table: "Groups",
                column: "RegisteredUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Groups_RegisteredUsers_RegisteredUserId",
                table: "Groups",
                column: "RegisteredUserId",
                principalTable: "RegisteredUsers",
                principalColumn: "Id");
        }
    }
}
