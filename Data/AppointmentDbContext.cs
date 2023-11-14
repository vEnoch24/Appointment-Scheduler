using Appointment_Scheduler.Model;
using Microsoft.EntityFrameworkCore;

namespace Appointment_Scheduler.Data
{
    public class AppointmentDbContext : DbContext
    {
        public AppointmentDbContext(DbContextOptions options) : base(options) 
        {

        }

        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<AppointmentUser> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure the one-to-many relationship
            modelBuilder.Entity<Appointment>()
                    .HasOne<AppointmentUser>()
                    .WithMany(u => u.Appointments)
                    .HasForeignKey(a => a.AppointmentUserId);
        }
    }
}
