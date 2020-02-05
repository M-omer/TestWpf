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
    public class TaskService : ITaskService
    {
        private string _baseurl = ConfigurationManager.AppSettings["baseUrl"];
        public TaskService()
        {

        }

        public async Task<List<Models.Task>> GetTasksAsync()
        {
            using (HttpClient client = new HttpClient())
            {
                List<Models.Task> tasks = new List<Models.Task>();
                HttpResponseMessage response = await client.GetAsync(_baseurl + "/Tasks/GetTasks");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    tasks = JsonConvert.DeserializeObject<List<Models.Task>>(json);
                }
                return tasks;
            }
        }

        public async Task<List<Models.Task>> GetTasksAsyncByID(int employeeID, int selectedWeek)
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response;
                List<Models.Task> tasks = new List<Models.Task>();
                try
                {
                     response = await client.GetAsync(_baseurl + "/Tasks/GetTaskByID?id=" + employeeID+"&wk="+ selectedWeek);
                }
                catch (Exception ex)
                {

                    throw;
                }
               
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    tasks = JsonConvert.DeserializeObject<List<Models.Task>>(json);
                }
                return tasks;
            }
        }

        public async Task<bool> SaveTasksItems(List<ItemGridSource> changes, int employeeID)
        {
            

            using (HttpClient client = new HttpClient())
            {
                var Json = JsonConvert.SerializeObject(changes);
                HttpContent httpContent = new StringContent(Json);
                httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/Json");
                List<Models.Task> tasks = new List<Models.Task>();
                HttpResponseMessage response = await client.PostAsync(_baseurl + "/Tasks/EditTasks?id="+ employeeID, httpContent);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    tasks = JsonConvert.DeserializeObject<List<Models.Task>>(json);
                    return true;
                }
                return false;
            }
        }

        public async Task<List<TimeSheet>> GetTasksTimesheets(int selectedWeek)
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response;
                List<TimeSheet> timesheet = new List<TimeSheet>();
                try
                {
                    response = await client.GetAsync(_baseurl + "/Tasks/GetSheetByWeek?wk=" + selectedWeek);
                }
                catch (Exception ex)
                {

                    throw;
                }

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    timesheet = JsonConvert.DeserializeObject<List<TimeSheet>>(json);
                }
                return timesheet;
            }
        }
    }
}
