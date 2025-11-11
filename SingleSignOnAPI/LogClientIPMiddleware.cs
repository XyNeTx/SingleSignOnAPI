using Serilog.Context;

namespace SingleSignOnAPI
{
    public class LogClientIPMiddleware
    {
        private readonly RequestDelegate _next;

        public LogClientIPMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var clientIp = context.Connection.RemoteIpAddress?.ToString();
            LogContext.PushProperty("ClientIP", clientIp);

            await _next(context);
        }

    }
}
