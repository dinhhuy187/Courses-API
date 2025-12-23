using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace courses_buynsell_api.Migrations
{
    /// <inheritdoc />
    public partial class IsBlock_for_conversation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsBlock",
                table: "Conversations",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsBlock",
                table: "Conversations");
        }
    }
}
