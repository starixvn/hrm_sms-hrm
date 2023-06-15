namespace GO.WebAPI.Middlewares
{
    using System;
    using System.Globalization;
    using System.Net;
    using System.Threading.Tasks;
    using Fanex.Logging;
    using Microsoft.AspNetCore.Http;
    using Newtonsoft.Json;

    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ILogger logger;

        public ErrorHandlerMiddleware(RequestDelegate next, ILogger logger)
        {
            this.next = next;
            this.logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            string message = string.Format(
                    CultureInfo.InvariantCulture,
                    "{0}\n\rSource: {1}\n\rDomain: {2}\n\rTargetSite: {3}\n\rStackTrace: {4}",
                    exception.Message,
                    exception.Source,
                    context.Request.Headers["Host"],
                    exception.TargetSite,
                    exception.StackTrace);
            await logger.ErrorAsync(message);

            if (IsAjaxRequest(context.Request))
            {
                var result = JsonConvert.SerializeObject(new { error = exception.Message });
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                await context.Response.WriteAsync(result);
            }
            else
            {
                context.Response.Redirect("/error/500");
            }
        }

        private static bool IsAjaxRequest(HttpRequest request)
           => string.Equals(request.Query["X-Requested-With"], "XMLHttpRequest", StringComparison.OrdinalIgnoreCase) ||
              string.Equals(request.Headers["X-Requested-With"], "XMLHttpRequest", StringComparison.OrdinalIgnoreCase);
    }
}