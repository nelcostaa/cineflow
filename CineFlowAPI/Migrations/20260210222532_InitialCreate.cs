using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cineflow.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Filmes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdTMDB = table.Column<int>(type: "int", nullable: false),
                    Titulo = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    TituloOriginal = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    IdiomaOriginal = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    DataLancamento = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Sinopse = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DuracaoMinutos = table.Column<int>(type: "int", nullable: true),
                    Genero = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: true),
                    PosterPath = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    BackdropPath = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    VoteAverage = table.Column<decimal>(type: "decimal(4,3)", precision: 4, scale: 3, nullable: true),
                    VoteCount = table.Column<int>(type: "int", nullable: true),
                    Popularity = table.Column<decimal>(type: "decimal(10,3)", precision: 10, scale: 3, nullable: true),
                    Adult = table.Column<bool>(type: "bit", nullable: false),
                    Video = table.Column<bool>(type: "bit", nullable: false),
                    AtualizadoEm = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Filmes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Salas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    CapacidadeTotal = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Salas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Sessoes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FilmeId = table.Column<int>(type: "int", nullable: false),
                    SalaId = table.Column<int>(type: "int", nullable: false),
                    HorarioInicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HorarioFim = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sessoes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sessoes_Filmes_FilmeId",
                        column: x => x.FilmeId,
                        principalTable: "Filmes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Sessoes_Salas_SalaId",
                        column: x => x.SalaId,
                        principalTable: "Salas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Ingressos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SessaoId = table.Column<int>(type: "int", nullable: false),
                    LugarMarcado = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    DataCompra = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ingressos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ingressos_Sessoes_SessaoId",
                        column: x => x.SessaoId,
                        principalTable: "Sessoes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Filmes_IdTMDB",
                table: "Filmes",
                column: "IdTMDB",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Ingressos_SessaoId_LugarMarcado",
                table: "Ingressos",
                columns: new[] { "SessaoId", "LugarMarcado" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sessoes_FilmeId",
                table: "Sessoes",
                column: "FilmeId");

            migrationBuilder.CreateIndex(
                name: "IX_Sessoes_SalaId_HorarioInicio_HorarioFim",
                table: "Sessoes",
                columns: new[] { "SalaId", "HorarioInicio", "HorarioFim" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Ingressos");

            migrationBuilder.DropTable(
                name: "Sessoes");

            migrationBuilder.DropTable(
                name: "Filmes");

            migrationBuilder.DropTable(
                name: "Salas");
        }
    }
}
