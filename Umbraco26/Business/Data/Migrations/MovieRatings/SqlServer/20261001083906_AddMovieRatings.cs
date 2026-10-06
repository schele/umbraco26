using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Umbraco26.Business.Data.Migrations.MovieRatings.SqlServer
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
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ImdbId = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Score = table.Column<double>(type: "float", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Comment = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CreatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
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
