using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using timesheet.data.Contracts;
using timesheet.data.Models;

namespace timesheet.data.Services
{
    public class EmployeeService : IEmployeeService
    {
        private string _baseurl = ConfigurationManager.AppSettings["baseUrl"];
        public EmployeeService()
        {

        }
        public async Task<List<Employee>> GetEmployees()
        {
            using (HttpClient client = new HttpClient())
            {
                List<Employee> employees = new List<Employee>();
                HttpResponseMessage response = await client.GetAsync(_baseurl + "/employee/getall");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    employees = JsonConvert.DeserializeObject<List<Employee>>(json);
                }
                return employees;
            }
        }



        //public async Task<bool> UpdateEmployee(int employeeID, int taskID, int selectedDay, int Hours)
        //{
        //    var obj = new Dictionary<string, string>
        //    {
        //        { "employeeID", employeeID.ToString()},
        //        { "taskID", taskID.ToString()},
        //        { "selectedDay", selectedDay.ToString()},
        //        { "Hours", Hours.ToString()}
        //    };
        //    using (HttpClient client = new HttpClient())
        //    {
        //        var Json = JsonConvert.SerializeObject(obj);
        //        HttpContent httpContent = new StringContent(Json);
        //        httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/Json");
               
        //        var response = await client.PostAsync(_baseurl + "/employee/updateEmployee", httpContent);
        //        if (response.IsSuccessStatusCode)
        //        {
        //            var json = await response.Content.ReadAsStringAsync();

        //        }
        //    }
        //    return true;
        //}
    }
}
