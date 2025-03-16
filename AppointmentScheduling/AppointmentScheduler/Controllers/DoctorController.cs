using AppointmentScheduler.Domain.Entities;
using AppointmentScheduler.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AppointmentScheduler.Controllers
{
	// urgent-hani: change this to use doctor service instead of infra.repo.
	// urgent-hani: figure out connecting to local db.
	[ApiController]
	[Route("api/[controller]")]
	public class DoctorController : ControllerBase
	{
		private readonly IDoctorRepository _doctorRepository;

		public DoctorController(IDoctorRepository doctorRepository)
		{
			_doctorRepository = doctorRepository;
		}

		[HttpGet("{id}")]
		public async Task<IActionResult> GetDoctorById(int id)
		{
			var doctor = await _doctorRepository.GetByIdAsync(id);
			if (doctor == null)
			{
				return NotFound();
			}
			return Ok(doctor);
		}
	}
}
