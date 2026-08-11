using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using QueueLess.API.Models;

namespace QueueLess.API.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public GlobalExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

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

            var statusCode = (int)HttpStatusCode.InternalServerError;
            var error = "InternalServerError";
            var message = exception.Message;
            var details = new List<string>();

            switch (exception)
            {
                case ArgumentException argEx:
                    statusCode = (int)HttpStatusCode.BadRequest;
                    error = "ValidationError";
                    message = "One or more validation errors occurred.";
                    details.Add(argEx.Message);
                    break;
                case InvalidOperationException invEx:
                    statusCode = (int)HttpStatusCode.Conflict;
                    error = "ConflictError";
                    message = invEx.Message;
                    break;
                case UnauthorizedAccessException authEx:
                    statusCode = (int)HttpStatusCode.Unauthorized;
                    error = "UnauthorizedError";
                    message = authEx.Message;
                    break;
                default:
                    message = "An unexpected error occurred on the server.";
                    details.Add(exception.Message);
                    break;
            }

            context.Response.StatusCode = statusCode;

            var response = new ApiErrorResponse
            {
                StatusCode = statusCode,
                Error = error,
                Message = message,
                Details = details
            };

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            var json = JsonSerializer.Serialize(response, options);
            return context.Response.WriteAsync(json);
        }
    }
}
