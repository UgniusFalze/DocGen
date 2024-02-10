using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DocsManager.Migrations
{
    /// <inheritdoc />
    public partial class InvoicesSeriesNumberu : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SeriesNumber",
                table: "Invoices",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SeriesNumber",
                table: "Invoices");
        }
    }
}
