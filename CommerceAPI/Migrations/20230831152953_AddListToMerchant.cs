using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CommerceAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddListToMerchant : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "ix_products_merchant_id",
                table: "products",
                column: "merchant_id");

            migrationBuilder.AddForeignKey(
                name: "fk_products_merchants_merchant_id",
                table: "products",
                column: "merchant_id",
                principalTable: "merchants",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_products_merchants_merchant_id",
                table: "products");

            migrationBuilder.DropIndex(
                name: "ix_products_merchant_id",
                table: "products");
        }
    }
}
