using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication2.Migrations
{
    /// <inheritdoc />
    public partial class BookPicture1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ProfilePicture",
                table: "Books",
                newName: "BookPicture");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "BookPicture",
                table: "Books",
                newName: "ProfilePicture");
        }
    }
}
