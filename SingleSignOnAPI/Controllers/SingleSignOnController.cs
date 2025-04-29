using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using SingleSignOnAPI.AppDbContext;
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
        public async Task<IActionResult> GetLogin(string system_name)
        {
            var UserName = User.FindFirst(ClaimTypes.Name)?.Value.ToString().Split("\\")[1];
            var DomainName = User.FindFirst(ClaimTypes.Name)?.Value.ToString().Split("\\")[0];

            if (string.IsNullOrEmpty(UserName) || string.IsNullOrEmpty(DomainName))
            {
                _activeDirectoryHelper.LogError
                    (UserName, _activeDirectoryHelper.GetHostNameByIp(system_name, UserName).Split(".")[0]
                    , _activeDirectoryHelper.GetIpAddress(system_name, UserName).ToString(), system_name, "Unauthorized");

                return Unauthorized();
            }

            var ipAddress = _activeDirectoryHelper.GetIpAddress(system_name, UserName);
            var ComputerName = _activeDirectoryHelper.GetHostNameByIp(system_name, UserName).Split(".")[0];
            var FullComputerName = _activeDirectoryHelper.GetHostNameByIp(system_name, UserName);

            try
            {
                //if (isLogin ?? false)
                //{
                //    var addObj = await _activeDirectoryHelper.AddUsesToTSql(UserName, ComputerName, ipAddress.ToString(), system_name);

                //    if (addObj != null)
                //    {
                //        return Ok(addObj);
                //    }
                //    else
                //    {
                //        return BadRequest();
                //    }
                //}
                //else
                //{
                var UserDetail = _flowContext.Employee.Where(x => x.EmployeeCode == UserName)
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
                }).FirstOrDefault();

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
                _activeDirectoryHelper.LogError(UserName, ComputerName, ipAddress.ToString(), system_name, ex.Message);
                Log.Error($"Error: {ex.Message} | User: {UserName} | Computer: {ComputerName} | IP: {ipAddress} | System: {system_name}");
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
                _activeDirectoryHelper.LogError(obj.Employee_Code, obj.Computer_Name, obj.Ip_Address, obj.System_Name, ex.Message);
                Log.Error($"Error: {ex.Message} | User: {obj.Employee_Code} | Computer: {obj.Computer_Name} | IP: {obj.Ip_Address} | System: {obj.System_Name}");
                return StatusCode(500, new
                {
                    status = "500",
                    response = "Internal Server Error",
                    message = ex.Message
                });
            }
        }

    }
}
