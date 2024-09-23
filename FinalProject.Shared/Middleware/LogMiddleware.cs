using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Credo.Core.Shared.Middleware
{
    public class LogMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<LogMiddleware> _logger;

        public LogMiddleware(RequestDelegate next, ILogger<LogMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            await _next(context);

            object? UserId;
            context.Items.TryGetValue("UserId", out UserId);
            string? userIdLog = String.IsNullOrEmpty(UserId?.ToString()) ? null : UserId.ToString();

            object? Email;
            context.Items.TryGetValue("Email", out Email);
             string? emailLog = String.IsNullOrEmpty(Email?.ToString()) ? "Not Logged In" : Email.ToString();

            object? Action;
            context.Items.TryGetValue("Action", out Action);
            string? actionLog = String.IsNullOrEmpty(Action?.ToString()) ? null : Action.ToString();

            #region Response

            #endregion

            #region Write Log

            if (context.Response.StatusCode == 200 || context.Response.StatusCode == 302)
            {
                _logger.LogInformation("{Type} - {StatusCode}, Path: '{Path}'" +
                                       ", {UserId}, {Email}, {Action}",
                                       "Successfully",
                                       context.Response.StatusCode,
                                       context.Request.Path,
                                       userIdLog,
                                       emailLog,
                                       actionLog);
            }

            #endregion
        }
    }
}
