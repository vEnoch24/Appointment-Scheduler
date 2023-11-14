using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Appointment_Scheduler.Migrations
{
    /// <inheritdoc />
    public partial class ModelModified : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "NewAppointmentId",
                table: "Appointments",
                nullable: false,
                defaultValueSql: "NEWID()"); // Use appropriate SQL for your database system

            migrationBuilder.DropPrimaryKey(
                name: "PK_Appointments",
                table: "Appointments");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Appointments",
                table: "Appointments",
                column: "NewAppointmentId");
        }

        // Add code to populate the new column with unique Guid values for each existing row

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NewAppointmentId",
                table: "Appointments");
        }
    }
}
