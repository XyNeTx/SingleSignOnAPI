using Azure.Core;
using Microsoft.AspNetCore.Cors;
using Newtonsoft.Json;
using Serilog;
using SingleSignOnAPI.AppDbContext;
using System.Net;

namespace SingleSignOnAPI
{
    [EnableCors("AllowSpecificOrigin")]
    public class ActiveDirectoryHelper
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly TSQLContext _tSQL;


        public ActiveDirectoryHelper(IHttpContextAccessor httpContextAccessor,TSQLContext tSQL)
        {
            _httpContextAccessor = httpContextAccessor;
            _tSQL = tSQL;
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

        public IPAddress GetIpAddress()
        {
            try
            {
                var _xForward = _httpContextAccessor.HttpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault() ?? _httpContextAccessor.HttpContext.Connection.RemoteIpAddress?.ToString();
                IPAddress ipAddress = IPAddress.Parse(_xForward);
                return ipAddress;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<T_SQL_License> AddUsesToTSql(string employeeCode, string computerName, string ipAddress,string system_name)
        {
            try
            {
                T_SQL_License addObj = new T_SQL_License
                {
                    F_System_Name = system_name,
                    F_UserID = employeeCode,
                    F_Host_Client = computerName,
                    F_IPAddress = ipAddress,
                    F_Update = DateTime.Now
                };

                _tSQL.T_SQL_License.Add(addObj);
                Log.Information($"Status : OK | Add User To T-Sql : {JsonConvert.SerializeObject(addObj)}");
                await _tSQL.SaveChangesAsync();

                return addObj;

            }
            catch (Exception ex)
            {
                return null;
                throw;
            }
        }

        public void LogError(string? employeeCode, string? computerName, string? ipAddress, string? system_name,string? message)
        {
            Log.Error($"Error: {message} | EmployeeCode: {employeeCode} | ComputerName: {computerName} | IpAddress: {ipAddress} | SystemName: {system_name}");
        }
    }
}
