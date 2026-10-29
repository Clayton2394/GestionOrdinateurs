using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestionOrdinateurs.API.Migrations
{
    /// <inheritdoc />
    public partial class RefactorOrdinateurModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reparations_Ordinateurs_OrdinateurId",
                table: "Reparations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Ordinateurs",
                table: "Ordinateurs");

            migrationBuilder.DropColumn(
                name: "RAM",
                table: "Ordinateurs");

            migrationBuilder.RenameTable(
                name: "Ordinateurs",
                newName: "Ordinateur");

            migrationBuilder.RenameColumn(
                name: "ID",
                table: "Ordinateur",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "Processeur",
                table: "Ordinateur",
                newName: "Caracteristiques_Processeur");

            migrationBuilder.RenameColumn(
                name: "Marque",
                table: "Ordinateur",
                newName: "Caracteristiques_Marque");

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

            migrationBuilder.AddColumn<int>(
                name: "Caracteristiques_TypeOrdi",
                table: "Ordinateur",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Ordinateur",
                table: "Ordinateur",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Reparations_Ordinateur_OrdinateurId",
                table: "Reparations",
                column: "OrdinateurId",
                principalTable: "Ordinateur",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reparations_Ordinateur_OrdinateurId",
                table: "Reparations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Ordinateur",
                table: "Ordinateur");

            migrationBuilder.DropColumn(
                name: "Caracteristiques_RamGo",
                table: "Ordinateur");

            migrationBuilder.DropColumn(
                name: "Caracteristiques_StockageGo",
                table: "Ordinateur");

            migrationBuilder.DropColumn(
                name: "Caracteristiques_TypeOrdi",
                table: "Ordinateur");

            migrationBuilder.RenameTable(
                name: "Ordinateur",
                newName: "Ordinateurs");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Ordinateurs",
                newName: "ID");

            migrationBuilder.RenameColumn(
                name: "Caracteristiques_Processeur",
                table: "Ordinateurs",
                newName: "Processeur");

            migrationBuilder.RenameColumn(
                name: "Caracteristiques_Marque",
                table: "Ordinateurs",
                newName: "Marque");

            migrationBuilder.AddColumn<string>(
                name: "RAM",
                table: "Ordinateurs",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Ordinateurs",
                table: "Ordinateurs",
                column: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_Reparations_Ordinateurs_OrdinateurId",
                table: "Reparations",
                column: "OrdinateurId",
                principalTable: "Ordinateurs",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
