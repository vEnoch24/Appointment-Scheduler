using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Appointment_Scheduler.Migrations
{
    /// <inheritdoc />
    public partial class AppointmentRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AppointmentUserId",
                table: "Appointments",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_AppointmentUserId",
                table: "Appointments",
                column: "AppointmentUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_Users_AppointmentUserId",
                table: "Appointments",
                column: "AppointmentUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_Users_AppointmentUserId",
                table: "Appointments");

            migrationBuilder.DropIndex(
                name: "IX_Appointments_AppointmentUserId",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "AppointmentUserId",
                table: "Appointments");
        }
    }
}
