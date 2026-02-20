using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BibliotecaVirtual.Migrations
{
    /// <inheritdoc />
    public partial class AddImagenUrl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImagenUrl",
                table: "Books",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImagenUrl",
                table: "Books");
        }
    }
}
