using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Trekify.API.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Treks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SerialNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    TrekName = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    State = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    TrekType = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    DifficultyLevel = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Season = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Duration = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Distance = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    MaxAltitude = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    TrekDescription = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: false),
                    Image = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    AgeGroup = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    GuideNeeded = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    SnowTrek = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    RecommendedGear = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Treks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Email = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    Password = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Treks");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
