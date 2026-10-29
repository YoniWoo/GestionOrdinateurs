using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestionOrdinateurs.API.Migrations
{
    /// <inheritdoc />
    public partial class RefactoringCaracteristiques : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Processeur",
                table: "Ordinateur",
                newName: "Caracteristiques_Processeur");

            migrationBuilder.RenameColumn(
                name: "Marque",
                table: "Ordinateur",
                newName: "Caracteristiques_Marque");

            migrationBuilder.RenameColumn(
                name: "RAM",
                table: "Ordinateur",
                newName: "Caracteristiques_TypeOrdi");

            migrationBuilder.AddColumn<int>(
                name: "Caracteristiques_RamGo",
                table: "Ordinateur",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Caracteristiques_StockageGo",
                table: "Ordinateur",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Caracteristiques_RamGo",
                table: "Ordinateur");

            migrationBuilder.DropColumn(
                name: "Caracteristiques_StockageGo",
                table: "Ordinateur");

            migrationBuilder.RenameColumn(
                name: "Caracteristiques_Processeur",
                table: "Ordinateur",
                newName: "Processeur");

            migrationBuilder.RenameColumn(
                name: "Caracteristiques_Marque",
                table: "Ordinateur",
                newName: "Marque");

            migrationBuilder.RenameColumn(
                name: "Caracteristiques_TypeOrdi",
                table: "Ordinateur",
                newName: "RAM");
        }
    }
}
