using Microsoft.AspNetCore.Cors;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Serilog;
using SingleSignOnAPI.AppDbContext;
using SingleSignOnAPI.Models;
using System.Net;

namespace SingleSignOnAPI
{
    [EnableCors("AllowSpecificOrigin")]
    public class ActiveDirectoryHelper
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly TSQLContext _tSQL;



        public ActiveDirectoryHelper(IHttpContextAccessor httpContextAccessor, TSQLContext tSQL)
        {
            _httpContextAccessor = httpContextAccessor;
            _tSQL = tSQL;
        }

        public string? GetHostNameByIp(string SystemName, string EmployeeCode)
        {

            string? hostName = "";
            IPAddress? ipAddress = new IPAddress(0);

            var logMessage = new VM_LOG()
            {
                //DateTime = DateTime.Now,
                Device = "",
                IP_Address = ipAddress.ToString(),
                Status = "OK",
                System = SystemName,
                User = EmployeeCode
            };

            try
            {
                var _xForward = _httpContextAccessor.HttpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault() ?? _httpContextAccessor.HttpContext.Connection.RemoteIpAddress?.ToString();
                ipAddress = IPAddress.Parse(_xForward);
                //var clientHostname = ipAddress != null ? Dns.GetHostEntry(ipAddress).HostName : "";
                var entry = System.Net.Dns.GetHostEntry(ipAddress);
                logMessage.IP_Address = ipAddress.ToString();
                logMessage.Device = hostName;
                return entry?.HostName ?? string.Empty;
                //return hostName;
            }
            catch (System.Net.Sockets.SocketException)
            {
                return string.Empty;

            }
            catch (Exception ex)
            {
                logMessage.Message = ex.InnerException.Message ?? ex.Message;
                logMessage.Status = "Error";
                Log.Error(JsonConvert.SerializeObject(ex),logMessage);
                return null;
            }
        }

        public IPAddress? GetIpAddress(string SystemName, string EmployeeCode)
        {

            var logMessage = new VM_LOG()
            {
                //DateTime = DateTime.Now,
                Device = "",
                IP_Address = "",
                Status = "OK",
                System = SystemName,
                User = EmployeeCode
            };

            try
            {
                var _xForward = _httpContextAccessor.HttpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault() ?? _httpContextAccessor.HttpContext.Connection.RemoteIpAddress?.ToString();
                IPAddress? ipAddress = IPAddress.Parse(_xForward);
                logMessage.IP_Address = ipAddress.ToString();
                return ipAddress;
            }
            catch (Exception ex)
            {
                logMessage.Message = ex.InnerException.Message ?? ex.Message;
                logMessage.Status = "Error";
                Log.Error(JsonConvert.SerializeObject(logMessage));
                return null;
            }
        }

        public async Task<T_SQL_License> AddUsesToTSql(string employeeCode, string computerName, string ipAddress, string system_name)
        {
            var logMessage = new VM_LOG()
            {
                //DateTime = DateTime.Now,
                Device = computerName,
                IP_Address = ipAddress,
                Status = "OK",
                System = system_name,
                User = employeeCode
            };

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

                var isExist = await _tSQL.T_SQL_License.AsNoTracking()
                    .AnyAsync(x => x.F_UserID == employeeCode
                    && x.F_Host_Client == computerName
                    && x.F_System_Name == system_name
                    && x.F_Update.Date == addObj.F_Update.Date);

                if (isExist)
                {
                    logMessage.Message = "User Already Exist";
                    Log.Information(JsonConvert.SerializeObject(logMessage));
                    return addObj;
                }
                else
                {
                    logMessage.Message = "Add User To T-Sql";
                    Log.Information(JsonConvert.SerializeObject(logMessage));
                    _tSQL.T_SQL_License.Add(addObj);
                    //Log.Information($"Status : OK | Add User To T-Sql : {JsonConvert.SerializeObject(addObj)}");
                    await _tSQL.SaveChangesAsync();

                    return addObj;
                }

            }
            catch (Exception ex)
            {
                logMessage.Message = ex.InnerException.Message ?? ex.Message;
                logMessage.Status = "Error";
                Log.Error(JsonConvert.SerializeObject(logMessage),logMessage);
                throw new Exception("Error: " + ex.Message);
            }
        }

        public void LogError(string? employeeCode, string? computerName, string? ipAddress, string? system_name, string? message)
        {
            var logMessage = new VM_LOG()
            {
                //DateTime = DateTime.Now,
                Device = computerName,
                IP_Address = ipAddress,
                Status = "Error",
                System = system_name,
                User = employeeCode,
                Message = message
            };

            Log.Error(JsonConvert.SerializeObject(logMessage));
        }
    }
}
