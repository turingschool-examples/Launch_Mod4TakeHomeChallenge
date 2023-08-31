using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CommerceAPI.Migrations
{
    /// <inheritdoc />
    public partial class editProductModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "price",
                table: "products");

            migrationBuilder.AddColumn<int>(
                name: "price_in_cents",
                table: "products",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "price_in_cents",
                table: "products");

            migrationBuilder.AddColumn<decimal>(
                name: "price",
                table: "products",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
