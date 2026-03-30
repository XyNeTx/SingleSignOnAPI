using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SingleSignOnAPI.AppDbContext;
using SingleSignOnAPI.Models.EmployeeInfo;

namespace SingleSignOnAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [EnableCors("AllowSpecificOrigin")]
    //[Authorize]
    public class EmployeeInfoController : ControllerBase
    {
        private WorkFlowContext _workFlowContext;
        public EmployeeInfoController(WorkFlowContext workFlowContext) 
        {
            _workFlowContext = workFlowContext;
        }
        [HttpGet]
        public async Task<IActionResult> Get(string employeeeCode)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(employeeeCode))
                {
                    return BadRequest(new
                    {
                        message = "Employee code is null"
                    });
                }

                var employeeInfo = new EmployeeInfo();
                employeeInfo = await _workFlowContext.EmployeeInfo
                    .FromSqlRaw($"SELECT Emp.EmployeeCode," +
                    $" Emp.DepartmentCode," +
                    $" Dep.DepartmentName," +
                    $" Emp.Name," +
                    $" Emp.Surname," +
                    $" Emp.Email," +
                    $" Position.PositionName," +
                    $" Position.PositionShortName," +
                    $" Dep.Initial" +
                    $" FROM [Organize].[Employee] Emp" +
                    $" INNER JOIN [WorkFlow].[Organize].[Position] Position " +
                    $" ON Emp.PositionCode = Position.PositionCode " +
                    $" INNER JOIN [WorkFlow].[Organize].[Department] Dep " +
                    $" ON Emp.DepartmentCode = Dep.DepartmentCode " +
                    $" AND Emp.Company = Dep.Company" +
                    $" WHERE Emp.EmployeeCode = '{employeeeCode}'")
                    .FirstOrDefaultAsync();

                if (employeeInfo == null)
                {
                    return NotFound(new
                    {
                        message = "The employee not found."
                    });
                }
                return Ok(new
                {
                   employeeInfo
                });
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                //var employeeInfo = new IEnumerable<EmployeeInfo>;
                var employeeInfo = _workFlowContext.EmployeeInfo
                    .FromSqlRaw($"SELECT Emp.EmployeeCode," +
                    $" Emp.DepartmentCode," +
                    $" Dep.DepartmentName," +
                    $" Emp.Name," +
                    $" Emp.Surname," +
                    $" Emp.Email," +
                    $" Position.PositionName," +
                    $" Position.PositionShortName," +
                    $" Dep.Initial" +
                    $" FROM [Organize].[Employee] Emp" +
                    $" INNER JOIN [WorkFlow].[Organize].[Position] Position " +
                    $" ON Emp.PositionCode = Position.PositionCode " +
                    $" INNER JOIN [WorkFlow].[Organize].[Department] Dep " +
                    $" ON Emp.DepartmentCode = Dep.DepartmentCode " +
                    $" AND Emp.Company = Dep.Company")
                    //$" WHERE Emp.EmployeeCode = '{employeeeCode}'")
                    .AsEnumerable();

                if (employeeInfo == null)
                {
                    return NotFound(new
                    {
                        message = "The employee not found."
                    });
                }
                return Ok(new
                {
                    employeeInfo
                });
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

    }

}
