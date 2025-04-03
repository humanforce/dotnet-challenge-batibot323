using AppointmentScheduler.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Numerics;

namespace AppointmentScheduler.Infrastructure
{
	public class AppointmentSchedulerDbContext : DbContext
	{
		public AppointmentSchedulerDbContext(DbContextOptions<AppointmentSchedulerDbContext> options)
			: base(options)
		{
		}

		public DbSet<Patient> Patients { get; set; }
		public DbSet<Doctor> Doctors { get; set; }
		public DbSet<Appointment> Appointments { get; set; }

		// todo-hani: not sure if we go db first or code first. i'd prefer to do sql queries for migration.
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			// Configure entity properties and relationships here if needed
		}
	}
}
