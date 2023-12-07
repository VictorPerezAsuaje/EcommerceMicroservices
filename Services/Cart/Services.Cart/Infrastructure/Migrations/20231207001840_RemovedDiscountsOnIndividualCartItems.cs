using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Services.Cart.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemovedDiscountsOnIndividualCartItems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiscountApplied",
                table: "CartItems");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "DiscountApplied",
                table: "CartItems",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
