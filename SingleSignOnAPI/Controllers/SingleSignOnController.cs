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
        public ActionResult<string> getLogin()
        {
            var UserName = User.FindFirst(ClaimTypes.Name)?.Value.ToString().Split("\\")[1];
            if (string.IsNullOrEmpty(UserName))
            {
                return Ok("Unauthorize");
            }

            var ComputerName = _activeDirectoryHelper.GetHostNameByIp().Split(".")[0];

            var response = new
            {
                UserName,
                ComputerName
            };

            return Ok(response);
        }
    }
}
