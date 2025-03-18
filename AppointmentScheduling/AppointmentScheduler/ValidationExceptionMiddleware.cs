using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace AppointmentScheduler.API.Middleware
{
	public class ValidationExceptionMiddleware
	{
		private readonly RequestDelegate _next;

		public ValidationExceptionMiddleware(RequestDelegate next)
		{
			_next = next;
		}

		// defer-hani: it seems to go here but i can't make it work. it's not throwing an exception?
		public async Task InvokeAsync(HttpContext context)
		{
			try
			{
				await _next(context);
			}
			catch (Exception ex)
			{
				await HandleExceptionAsync(context, ex);
			}
		}

		private static Task HandleExceptionAsync(HttpContext context, Exception exception)
		{
			context.Response.ContentType = "application/json";
			context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

			var problemDetails = new ValidationProblemDetails
			{
				Title = "One or more validation errors occurred.",
				Status = context.Response.StatusCode,
				Instance = context.Request.Path,
				Detail = "See the errors property for details.",
				Errors = new Dictionary<string, string[]>
				{
					{ "ValidationError", new[] { exception.Message } }
				}
			};

			var result = JsonSerializer.Serialize(problemDetails);
			return context.Response.WriteAsync(result);
		}
	}
}
