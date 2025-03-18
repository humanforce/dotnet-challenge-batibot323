using AppointmentScheduler.Infrastructure;
using AppointmentScheduler.Infrastructure.Repositories;
using AppointmentScheduler.Domain.Repositories;
using AppointmentScheduler.Domain.Services;
using Microsoft.EntityFrameworkCore;
using AppointmentScheduler.Domain.Entities;
using AppointmentScheduler.API.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Register the DbContext with the connection string
builder.Services.AddDbContext<AppointmentSchedulerDbContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
		sqlServerOptions => sqlServerOptions.EnableRetryOnFailure()));

// Register the repositories
builder.Services.AddScoped<IDoctorRepository, DoctorRepository>();
builder.Services.AddScoped<IPatientRepository, PatientRepository>();
builder.Services.AddScoped<IAppointmentRepository, AppointmentRepository>();

// Register the services
builder.Services.AddScoped<IDoctorService, DoctorService>();
builder.Services.AddScoped<IAppointmentService, AppointmentService>();
builder.Services.AddScoped<IPatientService, PatientService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

// Use custom middleware for handling validation errors
app.UseMiddleware<ValidationExceptionMiddleware>();

app.MapControllers();

app.Run();
