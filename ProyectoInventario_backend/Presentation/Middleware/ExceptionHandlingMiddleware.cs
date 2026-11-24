using System.Net;
using System.Text.Json;
using ProyectoInventario.Application.DTOs;

namespace ProyectoInventario.Presentation.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en {Method} {Path}", context.Request.Method, context.Request.Path);

                var (status, mensaje) = ex switch
                {
                    KeyNotFoundException => (HttpStatusCode.NotFound, ex.Message),
                    InvalidOperationException => (HttpStatusCode.Conflict, ex.Message),
                    ArgumentException => (HttpStatusCode.BadRequest, ex.Message),
                    _ => (HttpStatusCode.InternalServerError, "Ocurrió un error inesperado.")
                };

                context.Response.StatusCode = (int)status;
                context.Response.ContentType = "application/json";

                var response = new ErrorResponseDto(mensaje, ex.Message, context.TraceIdentifier);
                await context.Response.WriteAsync(JsonSerializer.Serialize(response));
            }
        }
    }
}
