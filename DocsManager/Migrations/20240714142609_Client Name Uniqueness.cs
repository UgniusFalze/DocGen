using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DocsManager.Migrations
{
    /// <inheritdoc />
    public partial class ClientNameUniqueness : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Clients_BuyerCode",
                table: "Clients",
                column: "BuyerCode",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Clients_BuyerCode",
                table: "Clients");
        }
    }
}
