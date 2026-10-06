using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Umbraco26.Business.Data.Migrations.MovieRatings.Sqlite
{
    /// <inheritdoc />
    public partial class AddMovieRatings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "movieFinderRating",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ImdbId = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Score = table.Column<double>(type: "REAL", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Comment = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    CreatedUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_movieFinderRating", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_movieFinderRating_ImdbId",
                table: "movieFinderRating",
                column: "ImdbId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "movieFinderRating");
        }
    }
}
