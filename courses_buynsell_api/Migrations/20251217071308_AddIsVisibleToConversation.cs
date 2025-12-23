using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace courses_buynsell_api.Migrations
{
    /// <inheritdoc />
    public partial class AddIsVisibleToConversation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsVisible",
                table: "Conversations",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsVisible",
                table: "Conversations");
        }
    }
}
