using AppointmentScheduler.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentScheduler.Domain.Services
{
	public class AppointmentService : IAppointmentService
	{
		Task<bool> IAppointmentService.CreateAppointment(Appointment appointment)
		{
			throw new NotImplementedException();
		}

		Task<Appointment> IAppointmentService.GetAppointmentById(int id)
		{
			throw new NotImplementedException();
		}
	}
}
