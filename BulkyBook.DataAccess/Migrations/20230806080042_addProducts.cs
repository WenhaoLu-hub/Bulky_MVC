using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BulkyBook.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class addProducts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Categories",
                type: "character varying(30)",
                maxLength: 30,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    ProductId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "text", nullable: false),
                    ISBN = table.Column<string>(type: "text", nullable: false),
                    Author = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    ListPrice = table.Column<double>(type: "double precision", nullable: false),
                    ListPrice50 = table.Column<double>(type: "double precision", nullable: false),
                    ListPrice100 = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.ProductId);
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "ProductId", "Author", "Description", "ISBN", "ListPrice", "ListPrice100", "ListPrice50", "Title" },
                values: new object[,]
                {
                    { 1, "William Somerset Maugham", "The Moon and Sixpence is a novel by W. Somerset Maugham, first published on 15 April 1919. It is told in episodic form by a first-person narrator providing a series of glimpses into the mind and soul of the central character, Charles Strickland, a middle-aged English stockbroker, who abandons his wife and children abruptly to pursue his desire to become an artist. The story is, in part, based on the life of the painter Paul Gauguin.", "9781598185218", 5.9900000000000002, 4.9900000000000002, 5.29, "The Moon and SixPence" },
                    { 2, "Ernest Hemingway", "The Old Man and the Sea is a novella written by the American author Ernest Hemingway in 1951 in Cayo Blanco (Cuba), and published in 1952.[1] It was the last major work of fiction written by Hemingway that was published during his lifetime. One of his most famous works, it tells the story of Santiago, an aging Cuban fisherman who struggles with a giant marlin far out in the Gulf Stream off the coast of Cuba.", "9780684830490", 5.9900000000000002, 4.9900000000000002, 5.29, "The Old Man and the Sea" },
                    { 3, "F. Scott Fitzgerald", "The Great Gatsby is a 1925 novel by American writer F. Scott Fitzgerald. Set in the Jazz Age on Long Island, near New York City, the novel depicts first-person narrator Nick Carraway's interactions with mysterious millionaire Jay Gatsby and Gatsby's obsession to reunite with his former lover, Daisy Buchanan.", "0743273567", 5.9900000000000002, 4.9900000000000002, 5.29, "The Great Gatsby" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Categories",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(30)",
                oldMaxLength: 30);
        }
    }
}
