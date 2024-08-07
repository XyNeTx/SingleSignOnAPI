using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
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

        public SingleSignOnController(ActiveDirectoryHelper activeDirectoryHelper)
        {
            _activeDirectoryHelper = activeDirectoryHelper;
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

            var response = new
            {
                UserName,
                DomainName,
                ComputerName,
                FullComputerName
            };

            return Ok(response);
        }
    }
}
