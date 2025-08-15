using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ProvaPub.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Numbers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Number = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Numbers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Value = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    OrderDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orders_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Customers",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Andres Turcotte" },
                    { 2, "Lynn Barrows" },
                    { 3, "Andy Deckow" },
                    { 4, "Pearl Powlowski" },
                    { 5, "Jordan Labadie" },
                    { 6, "Peter Keebler" },
                    { 7, "Bessie Von" },
                    { 8, "Shawn Gleason" },
                    { 9, "Meredith Hodkiewicz" },
                    { 10, "Cathy Wolff" },
                    { 11, "Marty Wilderman" },
                    { 12, "Hugo Rowe" },
                    { 13, "Essie Fisher" },
                    { 14, "Suzanne Ortiz" },
                    { 15, "Shaun Collins" },
                    { 16, "Rudy Mayert" },
                    { 17, "Lorene Padberg" },
                    { 18, "Felicia Johns" },
                    { 19, "Dale Streich" },
                    { 20, "Morris Quigley" }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Small Fresh Tuna" },
                    { 2, "Intelligent Metal Towels" },
                    { 3, "Handmade Metal Chips" },
                    { 4, "Handcrafted Soft Soap" },
                    { 5, "Rustic Concrete Tuna" },
                    { 6, "Handcrafted Concrete Chips" },
                    { 7, "Incredible Wooden Salad" },
                    { 8, "Gorgeous Metal Towels" },
                    { 9, "Small Plastic Pants" },
                    { 10, "Incredible Plastic Sausages" },
                    { 11, "Gorgeous Fresh Bacon" },
                    { 12, "Licensed Soft Gloves" },
                    { 13, "Intelligent Frozen Shirt" },
                    { 14, "Rustic Cotton Computer" },
                    { 15, "Intelligent Frozen Chips" },
                    { 16, "Ergonomic Fresh Ball" },
                    { 17, "Ergonomic Rubber Mouse" },
                    { 18, "Gorgeous Steel Salad" },
                    { 19, "Ergonomic Wooden Mouse" },
                    { 20, "Sleek Cotton Table" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Numbers_Number",
                table: "Numbers",
                column: "Number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_CustomerId",
                table: "Orders",
                column: "CustomerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Numbers");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Customers");
        }
    }
}
