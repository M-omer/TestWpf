using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using timesheet.data.Models;

namespace timesheet.data.Contracts
{
    public interface IEmployeeService
    {
        Task<List<Employee>> GetEmployees();
       
    }
}
