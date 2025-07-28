using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CadastroDePessoas.Infraestructure.Migrations
{
    [ExcludeFromCodeCoverage]
    /// <inheritdoc />
    public partial class initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Pessoas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Nome = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Sexo = table.Column<int>(type: "INTEGER", nullable: true),
                    Email = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    DataNascimento = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Naturalidade = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Nacionalidade = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    CPF = table.Column<string>(type: "TEXT", maxLength: 14, nullable: false),
                    Endereco = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    DataCadastro = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DataAtualizacao = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pessoas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Nome = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Senha = table.Column<string>(type: "TEXT", nullable: false),
                    DataCadastro = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Usuarios",
                columns: new[] { "Id", "DataCadastro", "Email", "Nome", "Senha" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "admin@exemplo.com", "Administrador", "$2a$12$ykxaUG2TOCFl1oCs0./Qv.yoR.vG4jA4u3HdYJO8C4fCyAabSnzXC" },
                    { new Guid("22222222-2222-2222-2222-222222222222"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "user@teste.com", "Usuário Teste", "$2a$12$w2ClXDQOe/ON4xWe7qxmTud4i72WEYJ04EOMbNjxRMJec9qiH1ggS" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Pessoas_CPF",
                table: "Pessoas",
                column: "CPF",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_Email",
                table: "Usuarios",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Pessoas");

            migrationBuilder.DropTable(
                name: "Usuarios");
        }
    }
}
