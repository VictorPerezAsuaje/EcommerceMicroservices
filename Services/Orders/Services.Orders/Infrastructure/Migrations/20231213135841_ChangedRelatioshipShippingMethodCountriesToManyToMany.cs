using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Services.Orders.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangedRelatioshipShippingMethodCountriesToManyToMany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShippingMethods_ShippingCountries_CountryName",
                table: "ShippingMethods");

            migrationBuilder.DropIndex(
                name: "IX_ShippingMethods_CountryName",
                table: "ShippingMethods");

            migrationBuilder.DropColumn(
                name: "CountryName",
                table: "ShippingMethods");

            migrationBuilder.CreateTable(
                name: "CountryShippingMethod",
                columns: table => new
                {
                    CountriesName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ShippingMethodName = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CountryShippingMethod", x => new { x.CountriesName, x.ShippingMethodName });
                    table.ForeignKey(
                        name: "FK_CountryShippingMethod_ShippingCountries_CountriesName",
                        column: x => x.CountriesName,
                        principalTable: "ShippingCountries",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CountryShippingMethod_ShippingMethods_ShippingMethodName",
                        column: x => x.ShippingMethodName,
                        principalTable: "ShippingMethods",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CountryShippingMethod_ShippingMethodName",
                table: "CountryShippingMethod",
                column: "ShippingMethodName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CountryShippingMethod");

            migrationBuilder.AddColumn<string>(
                name: "CountryName",
                table: "ShippingMethods",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingMethods_CountryName",
                table: "ShippingMethods",
                column: "CountryName");

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingMethods_ShippingCountries_CountryName",
                table: "ShippingMethods",
                column: "CountryName",
                principalTable: "ShippingCountries",
                principalColumn: "Name",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
