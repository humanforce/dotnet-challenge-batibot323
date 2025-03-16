using AppointmentScheduler.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentScheduler.Domain.Services
{
    public interface IAppointmentService
	{
		Task<Appointment> GetAppointmentById(int id);
		// returns true if created with no conflict, otherwise return false.
		Task<bool> CreateAppointment(Appointment appointment);
	}
}
