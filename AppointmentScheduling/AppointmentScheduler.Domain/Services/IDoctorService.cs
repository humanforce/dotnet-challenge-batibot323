using AppointmentScheduler.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AppointmentScheduler.Domain.Services
{
	public interface IDoctorService
	{
		Task<Doctor> GetDoctorByIdAsync(int id);
		Task<Doctor?> GetAvailableTimeSlotsAsync(int doctorId, DateTime date);
	}
}
