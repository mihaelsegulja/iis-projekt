using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IISNotionSearch.Repository.Migrations
{
    /// <inheritdoc />
    public partial class AddInTrash : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "InTrash",
                table: "NotionObjects",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InTrash",
                table: "NotionObjects");
        }
    }
}
