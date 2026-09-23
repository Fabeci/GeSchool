using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GeSchool.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSexeToEtudiant : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Sexe",
                table: "Etudiants",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Sexe",
                table: "Etudiants");
        }
    }
}
