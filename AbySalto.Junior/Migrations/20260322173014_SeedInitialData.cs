using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AbySalto.Junior.Migrations
{
    /// <inheritdoc />
    public partial class SeedInitialData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Orders",
                columns: new[] { "Id", "CustomerName", "Status", "OrderTime", "PaymentMethod", "DeliveryAddress", "ContactNumber", "Note", "Currency" },
                values: new object[] { 1, "Ivan Horvat", 0, new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc), "Cash", "Ilica 1, Zagreb", "0911234567", "Bez luka", "EUR" }
            );

            migrationBuilder.InsertData(
                table: "Orders",
                columns: new[] { "Id", "CustomerName", "Status", "OrderTime", "PaymentMethod", "DeliveryAddress", "ContactNumber", "Note", "Currency" },
                values: new object[] { 2, "Marija Kovač", 1, new DateTime(2026, 1, 2, 0, 0, 0, DateTimeKind.Utc), "Card", "Vukovarska 5, Zagreb", "0987654321", null, "EUR" }
            );

            migrationBuilder.InsertData(
                table: "OrderItems",
                columns: new[] { "OrderId", "Name", "Quantity", "Price" },
                values: new object[] { 1, "Pizza Margherita", 2, 8.50m }
            );

            migrationBuilder.InsertData(
                table: "OrderItems",
                columns: new[] { "OrderId", "Name", "Quantity", "Price" },
                values: new object[] { 1, "Cola", 2, 2.00m }
            );

            migrationBuilder.InsertData(
                table: "OrderItems",
                columns: new[] { "OrderId", "Name", "Quantity", "Price" },
                values: new object[] { 2, "Pasta Carbonara", 1, 10.00m }
            );

            migrationBuilder.InsertData(
                table: "OrderItems",
                columns: new[] { "OrderId", "Name", "Quantity", "Price" },
                values: new object[] { 2, "Tiramisu", 1, 5.00m }
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(table: "OrderItems", keyColumn: "OrderId", keyValue: 1);
            migrationBuilder.DeleteData(table: "OrderItems", keyColumn: "OrderId", keyValue: 2);
            migrationBuilder.DeleteData(table: "Orders", keyColumn: "Id", keyValue: 1);
            migrationBuilder.DeleteData(table: "Orders", keyColumn: "Id", keyValue: 2);
        }
    }
}
