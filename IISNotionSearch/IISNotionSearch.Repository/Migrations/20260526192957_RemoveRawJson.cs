using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IISNotionSearch.Repository.Migrations
{
    /// <inheritdoc />
    public partial class RemoveRawJson : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RawJson",
                table: "NotionObjects");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RawJson",
                table: "NotionObjects",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
