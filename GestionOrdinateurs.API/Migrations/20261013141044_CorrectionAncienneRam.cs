using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestionOrdinateurs.API.Migrations
{
    /// <inheritdoc />
    public partial class CorrectionAncienneRam : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                UPDATE "Ordinateur"
                SET "Caracteristiques_RamGo" = "Caracteristiques_TypeOrdi",
                    "Caracteristiques_TypeOrdi" = 1
                WHERE "Caracteristiques_RamGO" = 0
                    AND "Caracteristiques_TypeOrdi" > 2
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
