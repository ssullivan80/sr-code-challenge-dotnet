using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using challenge.Services;
using challenge.Models;

namespace challenge.Controllers
{
    [Route("api/employee")]
    public class EmployeeController : Controller
    {
        private readonly ILogger _logger;
        private readonly IEmployeeService _employeeService;

        public EmployeeController(ILogger<EmployeeController> logger, IEmployeeService employeeService)
        {
            _logger = logger;
            _employeeService = employeeService;
        }

        [HttpPost]
        public IActionResult CreateEmployee([FromBody] Employee employee)
        {
            _logger.LogDebug($"Received employee create request for '{employee.FirstName} {employee.LastName}'");

            _employeeService.Create(employee);

            return CreatedAtRoute("getEmployeeById", new { id = employee.EmployeeId }, employee);
        }

        [HttpGet("{id}", Name = "getEmployeeById")]
        public IActionResult GetEmployeeById(String id)
        {
            _logger.LogDebug($"Received employee get request for '{id}'");

            var employee = _employeeService.GetById(id);

            if (employee == null)
                return NotFound();

            return Ok(employee);
        }

        [HttpPut("{id}")]
        public IActionResult ReplaceEmployee(String id, [FromBody]Employee newEmployee)
        {
            _logger.LogDebug($"Recieved employee update request for '{id}'");

            var existingEmployee = _employeeService.GetById(id);
            if (existingEmployee == null)
                return NotFound();

            _employeeService.Replace(existingEmployee, newEmployee);

            return Ok(newEmployee);
        }

        [HttpGet("{id}/reports", Name = "getEmployeeReportingStructure")]
        public IActionResult GetEmployeeReportingStructure(String id)
        {
             _logger.LogDebug($"Received employee report get request for '{id}'");

            var employee = _employeeService.GetById(id);

            if (employee == null)
            {
                return NotFound();
            }

            int numOfReports = 0;
            employee = GetReportingEmployees(employee, ref numOfReports);

            return Ok(new ReportingStructure() { 
                Employee = employee,
                NumberOfReports = numOfReports
            });
        }

        private Employee GetReportingEmployees(Employee employee, ref int numOfReports)
        {
            if(employee.DirectReports != null && employee.DirectReports.Count > 0)
            {
                var referencedEmployees = new List<Employee>(employee.DirectReports.Count);
                numOfReports += employee.DirectReports.Count;

                foreach(var refEmp in employee.DirectReports)
                {
                    var referencedEmployee = _employeeService.GetById(refEmp.EmployeeId);
                    referencedEmployee = GetReportingEmployees(referencedEmployee, ref numOfReports);
                    referencedEmployees.Add(referencedEmployee);
                }
                employee.DirectReports = referencedEmployees;
            }
            return employee;
        }
    
       /* [HttpGet("{id}/compensation", Name = "getEmployeeCompensation")]
        public IActionResult GetEmployeeCompensation(String id)
        {
            _logger.LogDebug($"Received employee compensation get request for '{id}'");

            var employee = _employeeService.GetById(id);

            if (employee == null)
                return NotFound();
        }

        [HttpPost("{id}/compensation", Name = "setEmployeeCompensation")]
        public IActionResult SetEmployeeCompensation(String id, [FromBody]Compensation compensation)
        {
            _logger.LogDebug($"Received employee post compensation request for '{id}'");

            var employee = _employeeService.GetById(id);

            if (employee == null)
                return NotFound();
        }*/
    }
}
