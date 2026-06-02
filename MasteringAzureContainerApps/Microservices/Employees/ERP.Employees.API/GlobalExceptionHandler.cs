using Azure;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.Json;

namespace ERP.Employees.API
{
    public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger, IWebHostEnvironment environment) : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return false;
            }

            httpContext.Response.ContentType = "application/json";
            HttpStatusCode statusCode = HttpStatusCode.InternalServerError;
            string errorMessage = exception.Message;
            switch (exception)
            {
                case InvalidOperationException:
                    statusCode = HttpStatusCode.InternalServerError;
                    break;
                case UnauthorizedAccessException:
                    statusCode = HttpStatusCode.Unauthorized;
                    break;
                case BadHttpRequestException:
                case ArgumentNullException:
                case ArgumentException:
                case ValidationException:
                    statusCode = HttpStatusCode.BadRequest;
                    break;
                case CosmosException cosmosException:
                    statusCode = cosmosException.StatusCode;
                    errorMessage = this.CheckIsInDevelopment(cosmosException.Message);
                    break;
                case RequestFailedException requestFailedException:
                    statusCode = (HttpStatusCode)requestFailedException.Status;
                    errorMessage = this.CheckIsInDevelopment(requestFailedException.Message);
                    break;
                default:
                    statusCode = HttpStatusCode.InternalServerError;
                    break;
            }

            httpContext.Response.StatusCode = (int)statusCode;
            var problemDetails = new ProblemDetails
            {
                Title = this.CheckIsInDevelopment(exception.GetType().Name),
                Status = httpContext.Response.StatusCode,
                Instance = httpContext.Request.Path,
                Detail = errorMessage
            };

            problemDetails.Extensions["traceId"] = httpContext.TraceIdentifier;
            problemDetails.Extensions["stackTrace"] = this.CheckIsInDevelopment(exception.StackTrace);

            logger.LogError(exception, problemDetails.Title);

            string json = JsonSerializer.Serialize(problemDetails);
            httpContext.Response.ContentType = "application/json";
            await httpContext.Response.WriteAsync(json);
            return true;
        }

        private string CheckIsInDevelopment(string? message)
        {
            return environment.IsDevelopment() ? message : "An error occurred while processing your request.";
        }
    }
}