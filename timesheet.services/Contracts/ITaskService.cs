using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using timesheet.data.Models;

namespace timesheet.data.Contracts
{
    public interface ITaskService
    {
        Task<List<Models.Task>> GetTasksAsyncByID(int employeeID,int wk);
        Task<List<Models.Task>> GetTasksAsync();
    }
}
