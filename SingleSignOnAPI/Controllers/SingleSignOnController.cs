using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using SingleSignOnAPI.AppDbContext;
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

        public SingleSignOnController(ActiveDirectoryHelper activeDirectoryHelper,WorkFlowContext flowContext)
        {
            _activeDirectoryHelper = activeDirectoryHelper;
            _flowContext = flowContext;
        }

        [HttpGet]
        public IActionResult getLogin()
        {
            var UserName = User.FindFirst(ClaimTypes.Name)?.Value.ToString().Split("\\")[1];
            var DomainName = User.FindFirst(ClaimTypes.Name)?.Value.ToString().Split("\\")[0];

            if (string.IsNullOrEmpty(UserName))
            {
                return Unauthorized();
            }

            var ComputerName = _activeDirectoryHelper.GetHostNameByIp().Split(".")[0];
            var FullComputerName = _activeDirectoryHelper.GetHostNameByIp();
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


            var response = new
            {
                UserName,
                DomainName,
                ComputerName,
                FullComputerName,
                UserDetail
            };

            return Ok(response);
        }
    }
}
