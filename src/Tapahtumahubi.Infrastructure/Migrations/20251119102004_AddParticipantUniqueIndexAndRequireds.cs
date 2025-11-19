using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tapahtumahubi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddParticipantUniqueIndexAndRequireds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Participants_EventId",
                table: "Participants");

            migrationBuilder.CreateIndex(
                name: "IX_Participants_EventId_Email",
                table: "Participants",
                columns: new[] { "EventId", "Email" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Participants_EventId_Email",
                table: "Participants");

            migrationBuilder.CreateIndex(
                name: "IX_Participants_EventId",
                table: "Participants",
                column: "EventId");
        }
    }
}
