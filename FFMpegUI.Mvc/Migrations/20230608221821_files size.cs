using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FFMpegUI.Mvc.Migrations
{
    /// <inheritdoc />
    public partial class filessize : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ConvertedFileSize",
                table: "ProcessItems",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "SourceFileSize",
                table: "ProcessItems",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ConvertedFilesTotalSize",
                table: "Processes",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "SourceFilesTotalSize",
                table: "Processes",
                type: "bigint",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConvertedFileSize",
                table: "ProcessItems");

            migrationBuilder.DropColumn(
                name: "SourceFileSize",
                table: "ProcessItems");

            migrationBuilder.DropColumn(
                name: "ConvertedFilesTotalSize",
                table: "Processes");

            migrationBuilder.DropColumn(
                name: "SourceFilesTotalSize",
                table: "Processes");
        }
    }
}
