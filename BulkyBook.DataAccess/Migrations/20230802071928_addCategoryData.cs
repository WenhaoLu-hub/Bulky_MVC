using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BulkyBook.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class addCategoryData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "CategoryId", "DisplayOrder", "Name" },
                values: new object[,]
                {
                    { 1, 1, "Web Development" },
                    { 2, 2, "Application Development" },
                    { 3, 3, "Graphic Design" }
                }
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(table: "Categories", keyColumn: "CategoryId", keyValue: 1);

            migrationBuilder.DeleteData(table: "Categories", keyColumn: "CategoryId", keyValue: 2);

            migrationBuilder.DeleteData(table: "Categories", keyColumn: "CategoryId", keyValue: 3);
        }
    }
}
