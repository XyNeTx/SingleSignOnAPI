using Microsoft.AspNetCore.Cors;
using System.Net;

namespace SingleSignOnAPI
{
    [EnableCors("AllowSpecificOrigin")]
    public class ActiveDirectoryHelper
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ActiveDirectoryHelper(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        
        public string GetHostNameByIp()
        {
            try
            {
                var _xForward = _httpContextAccessor.HttpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault() ?? _httpContextAccessor.HttpContext.Connection.RemoteIpAddress?.ToString();
                IPAddress ipAddress = IPAddress.Parse(_xForward);
                string hostName = Dns.GetHostEntry(ipAddress).HostName;
                return hostName;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
