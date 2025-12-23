using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace courses_buynsell_api.Migrations
{
    /// <inheritdoc />
    public partial class Block_Table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Block_Users_SellerId",
                table: "Block");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Block",
                table: "Block");

            migrationBuilder.RenameTable(
                name: "Block",
                newName: "Blocks");

            migrationBuilder.RenameIndex(
                name: "IX_Block_SellerId",
                table: "Blocks",
                newName: "IX_Blocks_SellerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Blocks",
                table: "Blocks",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Blocks_Users_SellerId",
                table: "Blocks",
                column: "SellerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Blocks_Users_SellerId",
                table: "Blocks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Blocks",
                table: "Blocks");

            migrationBuilder.RenameTable(
                name: "Blocks",
                newName: "Block");

            migrationBuilder.RenameIndex(
                name: "IX_Blocks_SellerId",
                table: "Block",
                newName: "IX_Block_SellerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Block",
                table: "Block",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Block_Users_SellerId",
                table: "Block",
                column: "SellerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
