using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CadastroDePessoas.Infraestructure.Migrations
{
    [ExcludeFromCodeCoverage]
    /// <inheritdoc />
    public partial class atualizandoEntidadePessoasAddEntidadeEndereco : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Endereco",
                table: "Pessoas");

            migrationBuilder.AddColumn<string>(
                name: "Telefone",
                table: "Pessoas",
                type: "TEXT",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Enderecos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    CEP = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    Logradouro = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Numero = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Complemento = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Bairro = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Cidade = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Estado = table.Column<string>(type: "TEXT", maxLength: 2, nullable: false),
                    PessoaId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Enderecos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Enderecos_Pessoas_PessoaId",
                        column: x => x.PessoaId,
                        principalTable: "Pessoas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "Senha",
                value: "$2a$12$Fm13satlzTI/qiTH3ExOjuoccGxKlvc4X3v.A3EU6COcTrVvdqhQy");

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                column: "Senha",
                value: "$2a$12$OJ8nX5cnZvbvt638SUZlJuCTKjAP72mUcy02a3j0KRriFlOdSpHFW");

            migrationBuilder.CreateIndex(
                name: "IX_Enderecos_PessoaId",
                table: "Enderecos",
                column: "PessoaId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Enderecos");

            migrationBuilder.DropColumn(
                name: "Telefone",
                table: "Pessoas");

            migrationBuilder.AddColumn<string>(
                name: "Endereco",
                table: "Pessoas",
                type: "TEXT",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "Senha",
                value: "$2a$12$ykxaUG2TOCFl1oCs0./Qv.yoR.vG4jA4u3HdYJO8C4fCyAabSnzXC");

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                column: "Senha",
                value: "$2a$12$w2ClXDQOe/ON4xWe7qxmTud4i72WEYJ04EOMbNjxRMJec9qiH1ggS");
        }
    }
}
