using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using SingleSignOnAPI.AppDbContext;
using System.Net;
using System.Security.Claims;
//using System.Web.Http.Cors;

namespace SingleSignOnAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [EnableCors("AllowSpecificOrigin")]
    public class SingleSignOnController : ControllerBase
    {

        private readonly ActiveDirectoryHelper _activeDirectoryHelper;
        private readonly WorkFlowContext _flowContext;
        private readonly TSQLContext _tSQL;

        public SingleSignOnController(ActiveDirectoryHelper activeDirectoryHelper,WorkFlowContext flowContext, TSQLContext tSQL)
        {
            _activeDirectoryHelper = activeDirectoryHelper;
            _flowContext = flowContext;
            _tSQL = tSQL;
        }

        [HttpGet]
        public async Task<IActionResult> GetLogin()
        {
            try
            {
                var UserName = User.FindFirst(ClaimTypes.Name)?.Value.ToString().Split("\\")[1] == null ? "20234111" : User.FindFirst(ClaimTypes.Name)?.Value.ToString().Split("\\")[1];
                var DomainName = User.FindFirst(ClaimTypes.Name)?.Value.ToString().Split("\\")[0];
                if (string.IsNullOrEmpty(UserName))
                {
                    return Unauthorized();
                }

                var ComputerName = _activeDirectoryHelper.GetHostNameByIp().Split(".")[0];
                var FullComputerName = _activeDirectoryHelper.GetHostNameByIp();
                var ipAddress = _activeDirectoryHelper.GetIpAddress();
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

                await _activeDirectoryHelper.AddUsesToTSql(UserName, ComputerName, ipAddress.ToString());

                var response = new
                {
                    UserName,
                    DomainName,
                    ComputerName,
                    FullComputerName,
                    ipAddress,
                    UserDetail,
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        public IActionResult TestApi()
        {
            return Ok("Test API");
        }
    }
}
