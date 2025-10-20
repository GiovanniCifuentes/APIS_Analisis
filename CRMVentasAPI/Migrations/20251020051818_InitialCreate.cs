using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRMVentasAPI.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tareas_Oportunidades_OportunidadId",
                table: "Tareas");

            migrationBuilder.AlterColumn<int>(
                name: "OportunidadId",
                table: "Tareas",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Tareas_Oportunidades_OportunidadId",
                table: "Tareas",
                column: "OportunidadId",
                principalTable: "Oportunidades",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tareas_Oportunidades_OportunidadId",
                table: "Tareas");

            migrationBuilder.AlterColumn<int>(
                name: "OportunidadId",
                table: "Tareas",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Tareas_Oportunidades_OportunidadId",
                table: "Tareas",
                column: "OportunidadId",
                principalTable: "Oportunidades",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
