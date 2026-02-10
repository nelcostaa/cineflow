using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cineflow.Migrations
{
    /// <inheritdoc />
    public partial class FixFilmeDecimalPrecision : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "DuracaoMinutos",
                table: "Filmes",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<bool>(
                name: "Adult",
                table: "Filmes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "AtualizadoEm",
                table: "Filmes",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "BackdropPath",
                table: "Filmes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DataLancamento",
                table: "Filmes",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IdiomaOriginal",
                table: "Filmes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Popularity",
                table: "Filmes",
                type: "decimal(10,3)",
                precision: 10,
                scale: 3,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PosterPath",
                table: "Filmes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Sinopse",
                table: "Filmes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TituloOriginal",
                table: "Filmes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Video",
                table: "Filmes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "VoteAverage",
                table: "Filmes",
                type: "decimal(4,3)",
                precision: 4,
                scale: 3,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "VoteCount",
                table: "Filmes",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Adult",
                table: "Filmes");

            migrationBuilder.DropColumn(
                name: "AtualizadoEm",
                table: "Filmes");

            migrationBuilder.DropColumn(
                name: "BackdropPath",
                table: "Filmes");

            migrationBuilder.DropColumn(
                name: "DataLancamento",
                table: "Filmes");

            migrationBuilder.DropColumn(
                name: "IdiomaOriginal",
                table: "Filmes");

            migrationBuilder.DropColumn(
                name: "Popularity",
                table: "Filmes");

            migrationBuilder.DropColumn(
                name: "PosterPath",
                table: "Filmes");

            migrationBuilder.DropColumn(
                name: "Sinopse",
                table: "Filmes");

            migrationBuilder.DropColumn(
                name: "TituloOriginal",
                table: "Filmes");

            migrationBuilder.DropColumn(
                name: "Video",
                table: "Filmes");

            migrationBuilder.DropColumn(
                name: "VoteAverage",
                table: "Filmes");

            migrationBuilder.DropColumn(
                name: "VoteCount",
                table: "Filmes");

            migrationBuilder.AlterColumn<int>(
                name: "DuracaoMinutos",
                table: "Filmes",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
