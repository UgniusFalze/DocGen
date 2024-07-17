using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DocsManager.Migrations
{
    /// <inheritdoc />
    public partial class UserVatPayer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "VatCode",
                table: "Users",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VatCode",
                table: "Users");
        }
    }
}
