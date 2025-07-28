using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CadastroDePessoas.Infraestructure.Migrations
{
    public partial class TornarTelefoneOpcional : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Alterar a coluna Telefone para ser opcional
            migrationBuilder.AlterColumn<string>(
                name: "Telefone",
                table: "Pessoas",
                type: "TEXT",
                maxLength: 20,
                nullable: true, // Alterando para permitir nulos
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 20,
                oldNullable: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Reverter para a configuração anterior
            migrationBuilder.AlterColumn<string>(
                name: "Telefone",
                table: "Pessoas",
                type: "TEXT",
                maxLength: 20,
                nullable: false, // Revertendo para não permitir nulos
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 20,
                oldNullable: true);
        }
    }
}