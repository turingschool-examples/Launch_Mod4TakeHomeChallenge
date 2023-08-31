using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CommerceAPI.Migrations
{
    /// <inheritdoc />
    public partial class addDbSet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_product_merchants_merchant_id",
                table: "product");

            migrationBuilder.DropPrimaryKey(
                name: "pk_product",
                table: "product");

            migrationBuilder.RenameTable(
                name: "product",
                newName: "products");

            migrationBuilder.RenameIndex(
                name: "ix_product_merchant_id",
                table: "products",
                newName: "ix_products_merchant_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_products",
                table: "products",
                column: "id");

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

            migrationBuilder.DropPrimaryKey(
                name: "pk_products",
                table: "products");

            migrationBuilder.RenameTable(
                name: "products",
                newName: "product");

            migrationBuilder.RenameIndex(
                name: "ix_products_merchant_id",
                table: "product",
                newName: "ix_product_merchant_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_product",
                table: "product",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_product_merchants_merchant_id",
                table: "product",
                column: "merchant_id",
                principalTable: "merchants",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
