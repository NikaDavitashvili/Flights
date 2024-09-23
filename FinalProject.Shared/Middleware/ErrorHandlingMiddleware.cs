using FinalProject.Shared.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Net;

namespace Credo.Core.Shared.Middleware;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        Stopwatch sw = Stopwatch.StartNew();
        try
        {

            await _next(context);

        }
        catch (Exception ex)
        {
            sw.Stop();
            await HandleExceptionAsync(context, ex, sw);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception error, Stopwatch sw)
    {
        var response = context.Response;
        response.ContentType = "application/json";

        switch (error)
        {
            case ExternalServiceException e:
                _logger.LogError("{Type} - {StatusCode}, {StatusCodeTitle}  {Message}", nameof(ExternalServiceException), (int)HttpStatusCode.BadGateway, "ExternalServiceError", e.Message);
                response.StatusCode = (int)HttpStatusCode.BadGateway;
                break;
            case ValidationException e:
                _logger.LogError("{Type} - {StatusCode}, {StatusCodeTitle}  {Message}", nameof(ValidationException), (int)HttpStatusCode.BadRequest, "BadRequest", e.Message);
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                break;
            case NullReferenceException e:
                _logger.LogError("{Type} - {StatusCode}, {StatusCodeTitle}  {Message}", nameof(NullReferenceException), (int)HttpStatusCode.BadRequest, "BadRequest", e.Message);
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                break;
            case TimeoutException e:
                _logger.LogError("{Type} - {StatusCode}, {StatusCodeTitle}  {Message}", nameof(TimeoutException), (int)HttpStatusCode.RequestTimeout, "Request Timeout", e.Message);
                response.StatusCode = (int)HttpStatusCode.RequestTimeout;
                break;
            case ArgumentOutOfRangeException e:
                _logger.LogError("{Type} - {StatusCode}, {StatusCodeTitle}  {Message}", nameof(ArgumentOutOfRangeException), (int)HttpStatusCode.InternalServerError, "Argument Out Of Range", e.Message);
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                break;
            case Exception e:
                _logger.LogError("{Type} - {StatusCode}, {StatusCodeTitle}  {Message}", nameof(Exception), (int)HttpStatusCode.InternalServerError, "Internal Server Error", e.Message + " - INTERNAL ERROR");
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                break;
            default:
                _logger.LogError("{Type} - {StatusCode}, {StatusCodeTitle}  {Message}", nameof(Exception), (int)HttpStatusCode.InternalServerError, "Unknown Error", "Unknown Error");
                response.StatusCode = (int)HttpStatusCode.InternalServerError;

                break;

        }



        var result = JsonConvert.SerializeObject(new { Message = error.Message, StatusCode = response.StatusCode });
        await context.Response.WriteAsync(result);
    }
}
