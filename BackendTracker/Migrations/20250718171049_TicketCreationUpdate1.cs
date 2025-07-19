using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackendTracker.Migrations
{
    /// <inheritdoc />
    public partial class TicketCreationUpdate1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TicketFiles_Tickets_TicketId",
                table: "TicketFiles");

            migrationBuilder.AlterColumn<Guid>(
                name: "TicketId",
                table: "TicketFiles",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_TicketFiles_Tickets_TicketId",
                table: "TicketFiles",
                column: "TicketId",
                principalTable: "Tickets",
                principalColumn: "TicketId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TicketFiles_Tickets_TicketId",
                table: "TicketFiles");

            migrationBuilder.AlterColumn<Guid>(
                name: "TicketId",
                table: "TicketFiles",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddForeignKey(
                name: "FK_TicketFiles_Tickets_TicketId",
                table: "TicketFiles",
                column: "TicketId",
                principalTable: "Tickets",
                principalColumn: "TicketId");
        }
    }
}
