using ERPSystem.Core.Exceptions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace ERPSystem.Infrastructure.Middlewares;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
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
            _logger.LogError(ex, "An error occurred: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var response = new ErrorResponse();

        switch (exception)
        {
            case BusinessException ex:
                response.Message = ex.Message;
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                response.Details = "Business rule violation occurred.";
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                break;

            case ValidationException ex:
                response.Message = "Validation failed.";
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                response.Details = "One or more validation errors occurred.";
                response.Errors = ex.Errors;
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                break;

            case NotFoundException ex:
                response.Message = ex.Message;
                response.StatusCode = (int)HttpStatusCode.NotFound;
                response.Details = "The requested resource was not found.";
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                break;

            case UnauthorizedException ex:
                response.Message = ex.Message;
                response.StatusCode = (int)HttpStatusCode.Unauthorized;
                response.Details = "Authentication required.";
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                break;

            case ForbiddenException ex:
                response.Message = ex.Message;
                response.StatusCode = (int)HttpStatusCode.Forbidden;
                response.Details = "Access to this resource is forbidden.";
                context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                break;

            default:
                response.Message = "An internal server error occurred.";
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response.Details = exception.Message; // Production'da bunu kaldır
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                break;
        }

        var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(jsonResponse);
    }
}

// Error Response Model
public class ErrorResponse
{
    public int StatusCode { get; set; }
    public string Message { get; set; } = string.Empty;
    public string Details { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.Now;
    public Dictionary<string, string[]>? Errors { get; set; }
}