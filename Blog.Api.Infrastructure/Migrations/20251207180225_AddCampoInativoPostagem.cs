using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Blog.Api.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCampoInativoPostagem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Inativo",
                table: "Postagens",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Inativo",
                table: "Postagens");
        }
    }
}
