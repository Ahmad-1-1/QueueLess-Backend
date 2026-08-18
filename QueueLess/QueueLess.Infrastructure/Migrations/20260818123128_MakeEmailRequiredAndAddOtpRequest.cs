using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QueueLess.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MakeEmailRequiredAndAddOtpRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Remove the old filtered unique index created by AddEmailToUser migration
            migrationBuilder.DropIndex(
                name: "IX_Users_Email",
                table: "Users");

            // Make Email required and increase its maximum length
            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Users",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(150)",
                oldMaxLength: 150,
                oldNullable: true);

            // Create OTP requests table
            migrationBuilder.CreateTable(
                name: "OtpRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(
                        type: "uniqueidentifier",
                        nullable: false),

                    UserId = table.Column<Guid>(
                        type: "uniqueidentifier",
                        nullable: false),

                    OtpCode = table.Column<string>(
                        type: "nvarchar(max)",
                        nullable: false),

                    ExpiresAt = table.Column<DateTime>(
                        type: "datetime2",
                        nullable: false),

                    IsUsed = table.Column<bool>(
                        type: "bit",
                        nullable: false),

                    CreatedAt = table.Column<DateTime>(
                        type: "datetime2",
                        nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OtpRequests", x => x.Id);
                });

            // Create the final unique index on required Email
            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OtpRequests");

            migrationBuilder.DropIndex(
                name: "IX_Users_Email",
                table: "Users");

            // Restore the previous Email configuration
            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Users",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256);

            // Restore the old filtered unique index
            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true,
                filter: "[Email] IS NOT NULL");
        }
    }
}