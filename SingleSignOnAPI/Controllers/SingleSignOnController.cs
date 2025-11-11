using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SingleSignOnAPI.AppDbContext;
using System.Net;
using System.Security.Claims;
//using System.Web.Http.Cors;

namespace SingleSignOnAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [EnableCors("AllowSpecificOrigin")]
    [Authorize]
    public class SingleSignOnController : ControllerBase
    {

        private readonly ActiveDirectoryHelper _activeDirectoryHelper;
        private readonly WorkFlowContext _flowContext;

        public SingleSignOnController(ActiveDirectoryHelper activeDirectoryHelper, WorkFlowContext flowContext)
        {
            _activeDirectoryHelper = activeDirectoryHelper;
            _flowContext = flowContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetLogin(string? system_name)
        {
            string? UserName = null;
            string? DomainName = null;
            string? ComputerName = null;
            IPAddress? ipAddress = null;
            try
            {
                UserName = User.FindFirstValue(ClaimTypes.Name)?.ToString().Split("\\")[1];
                DomainName = User.FindFirstValue(ClaimTypes.Name)?.ToString().Split("\\")[0];
                ComputerName = UserName;
                if (string.IsNullOrEmpty(UserName) || string.IsNullOrEmpty(DomainName))
                {
                    _activeDirectoryHelper.LogError
                        (UserName, _activeDirectoryHelper.GetHostNameByIp(system_name, UserName).Split(".")[0]
                        , _activeDirectoryHelper.GetIpAddress(system_name, UserName).ToString(), system_name, "Unauthorized");

                    return Unauthorized();
                }

                ipAddress = _activeDirectoryHelper.GetIpAddress(system_name, UserName);
                var FullComputerName = _activeDirectoryHelper.GetHostNameByIp(system_name, UserName);
            
                if (string.IsNullOrWhiteSpace(FullComputerName))
                {
                    FullComputerName = UserName;
                }
                else ComputerName = FullComputerName.Split(".")[0];
          
                var UserDetail = await _flowContext.Employee.Where(x => x.EmployeeCode == UserName)
                .Select(x => new
                {
                    x.EmployeeCode,
                    x.DepartmentCode,
                    x.Company,
                    Title = x.Title == "นาย" ? "Mr." : x.Title == "นาง" ? "Mrs." : "Miss.",
                    x.Name,
                    x.Surname,
                    x.Email,
                    x.LocationCode
                }).FirstOrDefaultAsync();

                return Ok(new
                {
                    UserName,
                    DomainName,
                    ComputerName,
                    FullComputerName,
                    IpAddress = ipAddress.ToString(),
                    UserDetail
                });
                //}
            }
            catch (Exception ex)
            {
                _activeDirectoryHelper.LogError(UserName, ComputerName, ipAddress.ToString(), system_name, ex.InnerException.Message ?? ex.Message);
                //Log.Error($"Error: {ex.Message} | User: {UserName} | Computer: {ComputerName} | IP: {ipAddress} | System: {system_name}");
                return StatusCode(500, new
                {
                    status = "500",
                    response = "Internal Server Error",
                    message = ex.Message
                });
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> LoggedIn(VM_Post_Login obj)
        {
            try
            {
                var addObj = await _activeDirectoryHelper.AddUsesToTSql(obj.Employee_Code, obj.Computer_Name, obj.Ip_Address, obj.System_Name);

                if (addObj != null)
                {
                    return Ok(addObj);
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                _activeDirectoryHelper.LogError(obj.Employee_Code, obj.Computer_Name, obj.Ip_Address, obj.System_Name, ex.InnerException.Message ?? ex.Message);
                //Log.Error($"Error: {ex.Message} | User: {obj.Employee_Code} | Computer: {obj.Computer_Name} | IP: {obj.Ip_Address} | System: {obj.System_Name}");
                return StatusCode(500, new
                {
                    status = "500",
                    response = "Internal Server Error",
                    message = ex.Message
                });
            }
        }

        //[HttpPost]
        //[AllowAnonymous]
        //public async Task<IActionResult> GenerateUserDetail(VM_CmdPostData obj)
        //{
        //    try
        //    {
        //        var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "HostNameList");
        //        if (!Directory.Exists(folderPath))
        //        {
        //            Directory.CreateDirectory(folderPath);
        //        }

        //        var fileName = $"{obj.userName}.json";
        //        var filePath = Path.Combine(folderPath, fileName);
        //        var json = JsonSerializer.Serialize(obj, new JsonSerializerOptions { WriteIndented = true });
        //        await System.IO.File.WriteAllTextAsync(filePath, json);

        //        return Ok(new { Status = "Saved", File = fileName });

        //    }
        //    catch (Exception ex)
        //    {
        //        Log.Error($"Error: {ex.Message} | User: {obj.userName} | Computer: {obj.hostName}");
        //        return StatusCode(500, new
        //        {
        //            status = "500",
        //            response = "Internal Server Error",
        //            message = ex.Message
        //        });
        //    }
        //}

    }
}
