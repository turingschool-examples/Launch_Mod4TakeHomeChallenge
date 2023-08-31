using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CommerceAPI.Migrations
{
    /// <inheritdoc />
    public partial class MakeMerchantNullableForProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_products_merchants_merchant_id",
                table: "products");

            migrationBuilder.AlterColumn<int>(
                name: "merchant_id",
                table: "products",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "fk_products_merchants_merchant_id",
                table: "products",
                column: "merchant_id",
                principalTable: "merchants",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_products_merchants_merchant_id",
                table: "products");

            migrationBuilder.AlterColumn<int>(
                name: "merchant_id",
                table: "products",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "fk_products_merchants_merchant_id",
                table: "products",
                column: "merchant_id",
                principalTable: "merchants",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
