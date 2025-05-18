using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASP_SPR311.Migrations
{
    /// <inheritdoc />
    public partial class ProductAdded1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                schema: "ASP",
                table: "Products",
                type: "decimal(5,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "Stock",
                schema: "ASP",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Price",
                schema: "ASP",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Stock",
                schema: "ASP",
                table: "Products");
        }
    }
}
