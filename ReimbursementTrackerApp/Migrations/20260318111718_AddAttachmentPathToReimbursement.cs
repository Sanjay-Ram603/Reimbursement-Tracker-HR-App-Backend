using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReimbursementTrackerApp.Migrations
{
    /// <inheritdoc />
    public partial class AddAttachmentPathToReimbursement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AttachmentPath",
                table: "ReimbursementRequests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AttachmentPath",
                table: "ReimbursementRequests");
        }
    }
}
