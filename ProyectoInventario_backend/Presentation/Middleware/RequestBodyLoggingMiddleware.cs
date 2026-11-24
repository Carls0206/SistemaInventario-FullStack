using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace ProyectoInventario.Presentation.Middleware
{
    public class RequestBodyLoggingMiddleware
    {
        private static readonly HashSet<string> SensitiveKeys = new(StringComparer.OrdinalIgnoreCase)
        {
            "contrasena",
            "password",
            "token",
            "accessToken",
            "refreshToken",
            "secret",
            "key"
        };

        private readonly RequestDelegate _next;
        private readonly ILogger<RequestBodyLoggingMiddleware> _logger;

        public RequestBodyLoggingMiddleware(RequestDelegate next, ILogger<RequestBodyLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (!context.Request.Path.StartsWithSegments("/api"))
            {
                await _next(context);
                return;
            }

            try
            {
                await LogRequestAsync(context.Request);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "No se pudo registrar la trama entrante {Method} {Path}", context.Request.Method, context.Request.Path);
            }

            var originalBodyStream = context.Response.Body;
            await using var responseBodyStream = new MemoryStream();
            context.Response.Body = responseBodyStream;

            try
            {
                await _next(context);
            }
            finally
            {
                try
                {
                    await LogResponseAsync(context);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "No se pudo registrar la trama saliente {Method} {Path}", context.Request.Method, context.Request.Path);
                }

                responseBodyStream.Position = 0;
                await responseBodyStream.CopyToAsync(originalBodyStream);
                context.Response.Body = originalBodyStream;
            }
        }

        private async Task LogRequestAsync(HttpRequest request)
        {
            if (!HttpMethods.IsPost(request.Method) &&
                !HttpMethods.IsPut(request.Method) &&
                !HttpMethods.IsPatch(request.Method) &&
                !HttpMethods.IsDelete(request.Method))
            {
                return;
            }

            request.EnableBuffering();
            var body = await ReadStreamAsync(request.Body);
            if (string.IsNullOrWhiteSpace(body))
            {
                return;
            }

            _logger.LogInformation(
                "Trama entrante {Method} {Path}{Query} | ContentType: {ContentType} | Body: {Body}",
                request.Method,
                request.Path,
                request.QueryString,
                request.ContentType,
                SanitizeBody(body));
        }

        private async Task LogResponseAsync(HttpContext context)
        {
            context.Response.Body.Position = 0;
            var body = await ReadStreamAsync(context.Response.Body);

            if (string.IsNullOrWhiteSpace(body))
            {
                _logger.LogInformation(
                    "Trama saliente {Method} {Path}{Query} | StatusCode: {StatusCode}",
                    context.Request.Method,
                    context.Request.Path,
                    context.Request.QueryString,
                    context.Response.StatusCode);
                return;
            }

            _logger.LogInformation(
                "Trama saliente {Method} {Path}{Query} | StatusCode: {StatusCode} | ContentType: {ContentType} | Body: {Body}",
                context.Request.Method,
                context.Request.Path,
                context.Request.QueryString,
                context.Response.StatusCode,
                context.Response.ContentType,
                SanitizeBody(body));
        }

        private static async Task<string> ReadStreamAsync(Stream stream)
        {
            if (stream.CanSeek)
            {
                stream.Position = 0;
            }

            using var reader = new StreamReader(stream, Encoding.UTF8, detectEncodingFromByteOrderMarks: false, leaveOpen: true);
            var body = await reader.ReadToEndAsync();

            if (stream.CanSeek)
            {
                stream.Position = 0;
            }

            const int maxLength = 4000;
            if (body.Length > maxLength)
            {
                return body[..maxLength] + "...(truncado)";
            }

            return body;
        }

        private static string SanitizeBody(string body)
        {
            try
            {
                var node = JsonNode.Parse(body);
                if (node is null)
                {
                    return body;
                }

                RedactNode(node);
                return node.ToJsonString(new JsonSerializerOptions { WriteIndented = false });
            }
            catch
            {
                return body;
            }
        }

        private static void RedactNode(JsonNode node)
        {
            if (node is JsonObject obj)
            {
                foreach (var property in obj.ToList())
                {
                    if (property.Value is null)
                    {
                        continue;
                    }

                    if (SensitiveKeys.Contains(property.Key))
                    {
                        obj[property.Key] = "***REDACTED***";
                        continue;
                    }

                    RedactNode(property.Value);
                }
            }
            else if (node is JsonArray array)
            {
                foreach (var item in array)
                {
                    if (item is not null)
                    {
                        RedactNode(item);
                    }
                }
            }
        }
    }
}