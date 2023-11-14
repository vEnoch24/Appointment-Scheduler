using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Appointment_Scheduler.Migrations
{
    /// <inheritdoc />
    public partial class DropOldAppointmentId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AppointmentId",
                table: "Appointments");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "AppointmentId",
                table: "Appointments",
                nullable: false,
                defaultValue: 0L);
        }
    }
}
