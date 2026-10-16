using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MagnusCodex.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddVidaAdicional : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "VidaAdicional",
                table: "Personagens",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VidaAdicional",
                table: "Personagens");
        }
    }
}
