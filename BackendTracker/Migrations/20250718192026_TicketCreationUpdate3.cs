using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackendTracker.Migrations
{
    /// <inheritdoc />
    public partial class TicketCreationUpdate3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Tickets_AssigneeId",
                table: "Tickets",
                column: "AssigneeId");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_SubmitterId",
                table: "Tickets",
                column: "SubmitterId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_ApplicationUsers_AssigneeId",
                table: "Tickets",
                column: "AssigneeId",
                principalTable: "ApplicationUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_ApplicationUsers_SubmitterId",
                table: "Tickets",
                column: "SubmitterId",
                principalTable: "ApplicationUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_ApplicationUsers_AssigneeId",
                table: "Tickets");

            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_ApplicationUsers_SubmitterId",
                table: "Tickets");

            migrationBuilder.DropIndex(
                name: "IX_Tickets_AssigneeId",
                table: "Tickets");

            migrationBuilder.DropIndex(
                name: "IX_Tickets_SubmitterId",
                table: "Tickets");
        }
    }
}
