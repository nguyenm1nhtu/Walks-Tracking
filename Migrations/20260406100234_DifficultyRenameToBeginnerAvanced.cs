using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Walks.Migrations
{
    /// <inheritdoc />
    public partial class DifficultyRenameToBeginnerAvanced : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Difficulties",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "Beginner",
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldDefaultValue: "Easy");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Difficulties",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "Easy",
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldDefaultValue: "Beginner");
        }
    }
}
