using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Appointment_Scheduler.Migrations
{
    /// <inheritdoc />
    public partial class RenameGuidColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "NewAppointmentId",
                table: "Appointments",
                newName: "AppointmentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AppointmentId",
                table: "Appointments",
                newName: "NewAppointmentId");
        }
    }
}
