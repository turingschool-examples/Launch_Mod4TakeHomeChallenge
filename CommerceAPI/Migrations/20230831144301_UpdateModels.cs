using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CommerceAPI.Migrations
{
    /// <inheritdoc />
    public partial class UpdateModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_products_merchants_merchant_id",
                table: "products");

            migrationBuilder.DropIndex(
                name: "ix_products_merchant_id",
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "merchant_id",
                table: "products",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.CreateIndex(
                name: "ix_products_merchant_id",
                table: "products",
                column: "merchant_id");

            migrationBuilder.AddForeignKey(
                name: "fk_products_merchants_merchant_id",
                table: "products",
                column: "merchant_id",
                principalTable: "merchants",
                principalColumn: "id");
        }
    }
}
