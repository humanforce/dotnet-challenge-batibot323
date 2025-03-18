using AppointmentScheduler.Domain.Entities;
using System.Collections.Generic;

namespace AppointmentScheduler.Domain.DTOs
{
    public class AppointmentSummaryDto
    {
        public AppointmentStatusSummary Scheduled { get; set; }
        public AppointmentStatusSummary Completed { get; set; }
        public AppointmentStatusSummary Cancelled { get; set; }
    }

    public class AppointmentStatusSummary
    {
        public int Count { get; set; }
        public IEnumerable<Appointment> Appointments { get; set; }
    }
}
