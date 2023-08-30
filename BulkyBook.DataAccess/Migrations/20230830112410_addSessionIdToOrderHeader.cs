using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BulkyBook.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class addSessionIdToOrderHeader : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "name",
                table: "OrderHeaders",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "OderDate",
                table: "OrderHeaders",
                newName: "OrderDate");

            migrationBuilder.AddColumn<string>(
                name: "SessionId",
                table: "OrderHeaders",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SessionId",
                table: "OrderHeaders");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "OrderHeaders",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "OrderDate",
                table: "OrderHeaders",
                newName: "OderDate");
        }
    }
}
