using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IISNotionSearch.Repository.Migrations
{
    /// <inheritdoc />
    public partial class AddNotionObject : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NotionObjects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NotionId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ObjectType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Url = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Icon = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Cover = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastEditedTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    RawJson = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotionObjects", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NotionObjects");
        }
    }
}
