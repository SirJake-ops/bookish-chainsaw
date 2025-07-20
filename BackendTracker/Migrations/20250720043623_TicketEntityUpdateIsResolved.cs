using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackendTracker.Migrations
{
    /// <inheritdoc />
    public partial class TicketEntityUpdateIsResolved : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsResolved",
                table: "Tickets",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsResolved",
                table: "Tickets");
        }
    }
}
