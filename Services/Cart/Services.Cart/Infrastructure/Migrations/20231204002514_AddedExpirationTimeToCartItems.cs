using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Services.Cart.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedExpirationTimeToCartItems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ExpirationTime",
                table: "CartItems",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExpirationTime",
                table: "CartItems");
        }
    }
}
