using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineVoting_API.Migrations
{
    /// <inheritdoc />
    public partial class AddMultiChoiceLimit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "Polls");

            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "MultiChoiceLimit",
                table: "Polls",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Role",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "MultiChoiceLimit",
                table: "Polls");

            migrationBuilder.AddColumn<int>(
                name: "CreatedByUserId",
                table: "Polls",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
