using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FFMpegUI.Mvc.Migrations
{
    /// <inheritdoc />
    public partial class addedStatusMessageandSuccessfullytoProcess : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "StatusMessage",
                table: "Processes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Successfull",
                table: "Processes",
                type: "bit",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StatusMessage",
                table: "Processes");

            migrationBuilder.DropColumn(
                name: "Successfull",
                table: "Processes");
        }
    }
}
