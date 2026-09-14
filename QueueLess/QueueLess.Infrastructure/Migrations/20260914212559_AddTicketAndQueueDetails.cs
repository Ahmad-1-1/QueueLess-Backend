using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QueueLess.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTicketAndQueueDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "NotifyByEmail",
                table: "Tickets",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "NotifyByPush",
                table: "Tickets",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "QrCodeData",
                table: "Tickets",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TicketNumber",
                table: "Tickets",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "CountersOpen",
                table: "Services",
                type: "int",
                nullable: false,
                defaultValue: 1);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NotifyByEmail",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "NotifyByPush",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "QrCodeData",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "TicketNumber",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "CountersOpen",
                table: "Services");
        }
    }
}
