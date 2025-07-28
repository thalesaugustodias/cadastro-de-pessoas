using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CadastroDePessoas.Infraestructure.Migrations
{
    public partial class TornarComplementoOpcional : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Alterar a coluna Complemento para ser opcional
            migrationBuilder.AlterColumn<string>(
                name: "Complemento",
                table: "Enderecos",
                type: "TEXT",
                maxLength: 100,
                nullable: true, // Alterando para permitir nulos
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 100,
                oldNullable: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Reverter para a configuração anterior
            migrationBuilder.AlterColumn<string>(
                name: "Complemento",
                table: "Enderecos",
                type: "TEXT",
                maxLength: 100,
                nullable: false, // Revertendo para não permitir nulos
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 100,
                oldNullable: true);
        }
    }
}