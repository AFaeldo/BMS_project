using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BMS_project.Migrations
{
    /// <inheritdoc />
    public partial class AddProjectDocuments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "project_document",
                columns: table => new
                {
                    Document_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Project_ID = table.Column<int>(type: "int", nullable: false),
                    File_ID = table.Column<int>(type: "int", nullable: false),
                    Date_Added = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Description = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_project_document", x => x.Document_ID);
                    table.ForeignKey(
                        name: "FK_project_document_file_upload_File_ID",
                        column: x => x.File_ID,
                        principalTable: "file_upload",
                        principalColumn: "File_ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_project_document_project_Project_ID",
                        column: x => x.Project_ID,
                        principalTable: "project",
                        principalColumn: "Project_ID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_project_document_File_ID",
                table: "project_document",
                column: "File_ID");

            migrationBuilder.CreateIndex(
                name: "IX_project_document_Project_ID",
                table: "project_document",
                column: "Project_ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "project_document");
        }
    }
}
