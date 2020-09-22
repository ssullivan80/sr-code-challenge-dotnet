using System;
using System.Net;
using System.Net.Http;
using System.Text;
using challenge.Models;
using code_challenge.Tests.Integration.Extensions;
using code_challenge.Tests.Integration.Helpers;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace code_challenge.Tests.Integration
{
    [TestClass]
    public class CompensationControllerTests
    {
        private static HttpClient _httpClient;
        private static TestServer _testServer;

        [ClassInitialize]
        public static void InitializeClass(TestContext context)
        {
            _testServer = new TestServer(WebHost.CreateDefaultBuilder()
                .UseStartup<TestServerStartup>()
                .UseEnvironment("Development"));

            _httpClient = _testServer.CreateClient();
        }

        [ClassCleanup]
        public static void CleanUpTest()
        {
            _httpClient.Dispose();
            _testServer.Dispose();
        }

        [TestMethod]
        public void SetCompensation()
        {
            // Arrange
            var employeeId = "16a596ae-edd3-4847-99fe-c4518e82c86f";     
            var compensation = new Compensation()
            {
                Employee = new Employee()
                {
                    EmployeeId = employeeId,
                    Department = "Complaints",
                    FirstName = "Debbie",
                    LastName = "Downer",
                    Position = "Receiver",
                },
                Salary = 20.00M,
                EffectiveDate = DateTime.Now
            };            

            
            var requestContent = new JsonSerialization().ToJson(compensation);

            // Execute
            var postRequestTask = _httpClient.PostAsync("api/compensation/",
               new StringContent(requestContent, Encoding.UTF8, "application/json"));
            var response = postRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
            var compensationReturned = response.DeserializeContent<Compensation>();
            Assert.AreEqual(employeeId, compensationReturned.Employee.EmployeeId);            
        }

        [TestMethod]
        public void GetCompensation()
        {
            // Arrange
            var employeeId = "16a596ae-edd3-4847-99fe-c4518e82c86f";     
            var compensation = new Compensation()
            {
                Employee = new Employee()
                {
                    EmployeeId = employeeId,
                    Department = "Complaints",
                    FirstName = "Debbie",
                    LastName = "Downer",
                    Position = "Receiver",
                },
                Salary = 20.00M,
                EffectiveDate = DateTime.Now
            };            

            
            var requestContent = new JsonSerialization().ToJson(compensation);

            // Execute
            var postRequestTask = _httpClient.PostAsync("api/compensation",
               new StringContent(requestContent, Encoding.UTF8, "application/json"));
            var firstResponse = postRequestTask.Result;
            

            // Execute
            var getRequestTask = _httpClient.GetAsync($"api/compensation/{employeeId}");
            var response = getRequestTask.Result;


            // Assert
            Assert.AreEqual(HttpStatusCode.Created, firstResponse.StatusCode);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var compensationReturned = response.DeserializeContent<Compensation>();
            Assert.AreEqual(employeeId, compensationReturned.Employee.EmployeeId);            
        }            
    }
}