using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TvMaze.Database.Migrations.Sqlite.TvMazeDb
{
    /// <inheritdoc />
    public partial class InitialTvMazeDbMigrationSqlite : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Show",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TvMazeId = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Show", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Cast",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TvMazeId = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Birthday = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ShowId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cast", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cast_Show_ShowId",
                        column: x => x.ShowId,
                        principalTable: "Show",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Cast_ShowId_Name",
                table: "Cast",
                columns: new[] { "ShowId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Show_Name",
                table: "Show",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Show_TvMazeId",
                table: "Show",
                column: "TvMazeId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Cast");

            migrationBuilder.DropTable(
                name: "Show");
        }
    }
}
