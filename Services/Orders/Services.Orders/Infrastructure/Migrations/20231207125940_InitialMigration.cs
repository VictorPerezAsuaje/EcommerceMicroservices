using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Services.Orders.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DiscountCodes",
                columns: table => new
                {
                    Code = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<double>(type: "float", maxLength: 4, nullable: false),
                    NumberOfUses = table.Column<int>(type: "int", nullable: false),
                    MaxUsage = table.Column<int>(type: "int", nullable: false),
                    MaxUsagePerClient = table.Column<int>(type: "int", nullable: false),
                    ValidUntil = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AssociatedClientId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscountCodes", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "OrderHistory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderHistory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OrderStatus",
                columns: table => new
                {
                    Alias = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderStatus", x => x.Alias);
                });

            migrationBuilder.CreateTable(
                name: "PaymentMethods",
                columns: table => new
                {
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentMethods", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "ShippingCountries",
                columns: table => new
                {
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShippingCountries", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "OrderHistoryItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderHistoryId = table.Column<int>(type: "int", nullable: false),
                    OrderStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ChangeDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderHistoryItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderHistoryItems_OrderHistory_OrderHistoryId",
                        column: x => x.OrderHistoryId,
                        principalTable: "OrderHistory",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ClientShippingData",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ShippingFirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ShippingLastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ShippingAddress_CountryName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ShippingAddress_MajorDivision = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    ShippingAddress_MajorDivisionCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true, defaultValue: "N/A"),
                    ShippingAddress_Locality = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    ShippingAddress_MinorDivision = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true, defaultValue: "N/A"),
                    ShippingAddress_Street = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    ShippingAddress_StreetNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false, defaultValue: "N/A"),
                    ShippingAddress_HouseNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false, defaultValue: "N/A"),
                    ShippingAddress_PostalCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false, defaultValue: "N/A"),
                    ShippingAddress_Latitude = table.Column<double>(type: "float", nullable: false),
                    ShippingAddress_Longitude = table.Column<double>(type: "float", nullable: false),
                    ShippingAddress_ExtraDetails = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientShippingData", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClientShippingData_ShippingCountries_ShippingAddress_CountryName",
                        column: x => x.ShippingAddress_CountryName,
                        principalTable: "ShippingCountries",
                        principalColumn: "Name");
                });

            migrationBuilder.CreateTable(
                name: "ShippingMethods",
                columns: table => new
                {
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CountryName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ApplicableFees = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShippingMethods", x => x.Name);
                    table.ForeignKey(
                        name: "FK_ShippingMethods_ShippingCountries_CountryName",
                        column: x => x.CountryName,
                        principalTable: "ShippingCountries",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ShippingFirstName = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    ShippingLastName = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    ShippingAddress_CountryName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ShippingAddress_MajorDivision = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    ShippingAddress_MajorDivisionCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true, defaultValue: "N/A"),
                    ShippingAddress_Locality = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    ShippingAddress_MinorDivision = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true, defaultValue: "N/A"),
                    ShippingAddress_Street = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    ShippingAddress_StreetNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false, defaultValue: "N/A"),
                    ShippingAddress_HouseNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false, defaultValue: "N/A"),
                    ShippingAddress_PostalCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false, defaultValue: "N/A"),
                    ShippingAddress_Latitude = table.Column<double>(type: "float", nullable: false),
                    ShippingAddress_Longitude = table.Column<double>(type: "float", nullable: false),
                    ShippingAddress_ExtraDetails = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ShippingMethod = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ShippingFees = table.Column<double>(type: "float", maxLength: 2000, nullable: false),
                    DiscountCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CurrentOrderStatusId = table.Column<int>(type: "int", nullable: false),
                    HistoryId = table.Column<int>(type: "int", nullable: false),
                    SubTotal = table.Column<double>(type: "float", nullable: false),
                    TaxApplied = table.Column<double>(type: "float", nullable: false),
                    Total = table.Column<double>(type: "float", nullable: false),
                    OrderDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ShippingMethodName = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    PaymentMethodName = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orders_OrderHistoryItems_CurrentOrderStatusId",
                        column: x => x.CurrentOrderStatusId,
                        principalTable: "OrderHistoryItems",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Orders_OrderHistory_HistoryId",
                        column: x => x.HistoryId,
                        principalTable: "OrderHistory",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Orders_PaymentMethods_PaymentMethodName",
                        column: x => x.PaymentMethodName,
                        principalTable: "PaymentMethods",
                        principalColumn: "Name");
                    table.ForeignKey(
                        name: "FK_Orders_ShippingCountries_ShippingAddress_CountryName",
                        column: x => x.ShippingAddress_CountryName,
                        principalTable: "ShippingCountries",
                        principalColumn: "Name");
                    table.ForeignKey(
                        name: "FK_Orders_ShippingMethods_ShippingMethodName",
                        column: x => x.ShippingMethodName,
                        principalTable: "ShippingMethods",
                        principalColumn: "Name");
                });

            migrationBuilder.CreateTable(
                name: "ClientDiscountCodes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<double>(type: "float", nullable: false),
                    ClientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UsageDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientDiscountCodes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClientDiscountCodes_DiscountCodes_Code",
                        column: x => x.Code,
                        principalTable: "DiscountCodes",
                        principalColumn: "Code");
                    table.ForeignKey(
                        name: "FK_ClientDiscountCodes_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "OrderItem",
                columns: table => new
                {
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<double>(type: "float", nullable: false),
                    Amount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItem", x => new { x.OrderId, x.ProductId });
                    table.ForeignKey(
                        name: "FK_OrderItem_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClientDiscountCodes_Code",
                table: "ClientDiscountCodes",
                column: "Code");

            migrationBuilder.CreateIndex(
                name: "IX_ClientDiscountCodes_OrderId",
                table: "ClientDiscountCodes",
                column: "OrderId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ClientShippingData_ShippingAddress_CountryName",
                table: "ClientShippingData",
                column: "ShippingAddress_CountryName");

            migrationBuilder.CreateIndex(
                name: "IX_OrderHistoryItems_OrderHistoryId",
                table: "OrderHistoryItems",
                column: "OrderHistoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_CurrentOrderStatusId",
                table: "Orders",
                column: "CurrentOrderStatusId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_HistoryId",
                table: "Orders",
                column: "HistoryId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_PaymentMethodName",
                table: "Orders",
                column: "PaymentMethodName");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_ShippingAddress_CountryName",
                table: "Orders",
                column: "ShippingAddress_CountryName");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_ShippingMethodName",
                table: "Orders",
                column: "ShippingMethodName");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingMethods_CountryName",
                table: "ShippingMethods",
                column: "CountryName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClientDiscountCodes");

            migrationBuilder.DropTable(
                name: "ClientShippingData");

            migrationBuilder.DropTable(
                name: "OrderItem");

            migrationBuilder.DropTable(
                name: "OrderStatus");

            migrationBuilder.DropTable(
                name: "DiscountCodes");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "OrderHistoryItems");

            migrationBuilder.DropTable(
                name: "PaymentMethods");

            migrationBuilder.DropTable(
                name: "ShippingMethods");

            migrationBuilder.DropTable(
                name: "OrderHistory");

            migrationBuilder.DropTable(
                name: "ShippingCountries");
        }
    }
}
