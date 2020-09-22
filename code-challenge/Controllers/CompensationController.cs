using System;
using challenge.Models;
using challenge.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace challenge.Controllers
{
    [Route("api/compensation")]
    public class CompensationController: Controller
    {
        private readonly ILogger _logger;
        private readonly IEmployeeService _employeeService;    
        public CompensationController(ILogger<CompensationController> logger, IEmployeeService employeeService)
        {
            _logger = logger;
            _employeeService = employeeService;
        }

        [HttpGet("{id}", Name = "getEmployeeCompensation")]
        public IActionResult GetEmployeeCompensation(String id)
        {
            _logger.LogDebug($"Received employee compensation get request for '{id}'");

            var employee = _employeeService.GetById(id);

            if (employee == null)
                return NotFound();

            Compensation compensation = _employeeService.GetCompensationByEmployeeId(id);

            if(compensation == null)
            {
                return BadRequest();
            }

            return Ok(compensation);
        }

        [HttpPost]
        public IActionResult SetEmployeeCompensation([FromBody]Compensation compensation)
        {
            _logger.LogDebug($"Received employee post compensation request for '{compensation.Employee.EmployeeId}'");

            var employee = _employeeService.GetById(compensation.Employee.EmployeeId);

            if (employee == null)
                return NotFound();

            _employeeService.AddCompensation(compensation);

            return CreatedAtRoute("getEmployeeCompensation", new { id = employee.EmployeeId }, compensation);;
        }
    }
}