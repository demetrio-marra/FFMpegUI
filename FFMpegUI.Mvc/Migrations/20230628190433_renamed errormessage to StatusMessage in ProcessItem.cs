using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FFMpegUI.Mvc.Migrations
{
    /// <inheritdoc />
    public partial class renamederrormessagetoStatusMessageinProcessItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ErrorMessage",
                table: "ProcessItems",
                newName: "StatusMessage");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StatusMessage",
                table: "ProcessItems",
                newName: "ErrorMessage");
        }
    }
}
