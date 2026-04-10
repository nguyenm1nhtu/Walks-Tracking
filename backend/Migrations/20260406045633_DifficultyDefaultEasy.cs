using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Walks.Migrations
{
    /// <inheritdoc />
    public partial class DifficultyDefaultEasy : Migration
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
                defaultValue: "Easy",
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);
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
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldDefaultValue: "Easy");
        }
    }
}
