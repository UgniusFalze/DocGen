using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DocsManager.Migrations
{
    /// <inheritdoc />
    public partial class Uniqueness : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Invoices_InvoiceUserId",
                table: "Invoices");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_InvoiceUserId_SeriesNumber",
                table: "Invoices",
                columns: new[] { "InvoiceUserId", "SeriesNumber" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Invoices_InvoiceUserId_SeriesNumber",
                table: "Invoices");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_InvoiceUserId",
                table: "Invoices",
                column: "InvoiceUserId");
        }
    }
}
