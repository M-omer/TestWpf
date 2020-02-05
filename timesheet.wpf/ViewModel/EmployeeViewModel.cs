using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using timesheet.core.Base;
using timesheet.core.Singleton;
using timesheet.data.Models;
using timesheet.data.Services;
using Newtonsoft.Json;
using System.Reflection;
using timesheet.wpf.Commands;
using System.Windows.Controls;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using Task = System.Threading.Tasks.Task;
using System.Globalization;

namespace timesheet.wpf.ViewModel
{

    public class EmployeeViewModel : BaseViewModel
    {
        private EmployeeService _employeeService;
        private TaskService _taskService;
        public RelayCommand LoadedCommand { get; set; }
        public RelayCommand EditCommand { get; set; }
        public RelayCommand SelectedWeekChangedCommand { get; set; }
        public RelayCommand SelectedEmpChangedCommand { get; set; }
        public RelayCommand DailySelectionChangedCommand { get; set; }
        public EmployeeViewModel()
        {
            _employeeService = (EmployeeService)SingletonInstances.GetEmployeeService(typeof(EmployeeService));
            _taskService = (TaskService)SingletonInstances.GetEmployeeService(typeof(TaskService));
            LoadedCommand = new RelayCommand(async parm => await GetTasksByID(parm), canExecute);
            EditCommand = new RelayCommand(async parm => await EditTasksAsync(parm), canExecute);
            SelectedWeekChangedCommand = new RelayCommand(async parm => await WeekChangedAsync(parm), canExecute);
            SelectedEmpChangedCommand = new RelayCommand(EmpChanged, canExecute);
            DailySelectionChangedCommand = new RelayCommand(parm => DailyParmChanged(parm), canExecute);
            System.Threading.Tasks.Task.Run(new Action(OnLoaded));
        }

        private void DailyParmChanged(object parm)
        {
         
            var clone = DeepCopy(Originaltimesheets.Where(e => e.Employee.EmployeeID == SelectedEmployee.EmployeeID)).ToList();
            if (parm != null)
            {
                if (parm.GetType() == typeof(data.Models.Task))
                {
                    DayOfWeek day = SelectedDay;
                    var num = ((int)Enum.Parse(typeof(DayOfWeek), day.ToString()));
                    try
                    {
                        TimeSheets = clone.Where(i => i.Task.TaskID == SelectedTask.TaskID && i.Day.Day == num.ToString()).FirstOrDefault();
                    }
                    catch (Exception ex)
                    {

                        throw;
                    }

                }
                if (parm.GetType() == typeof(DayOfWeek))
                {
                    if (SelectedTask != null)
                    {
                        DayOfWeek day = SelectedDay;
                        var num = ((int)Enum.Parse(typeof(DayOfWeek), day.ToString()));
                        try
                        {
                            TimeSheets = clone.Where(i => i.Task.TaskID == SelectedTask.TaskID && i.Day.Day == num.ToString()).FirstOrDefault();
                        }
                        catch (Exception ex)
                        {

                            throw;
                        }
                    }

                }
            }
        }


        private async void EmpChanged(object parameter)
        {
            try
            {
                TimeSheets = new TimeSheet();
                await GetTasksByID(true);
            }
            catch (Exception)
            {

                throw;
            }

        }



        private async Task EditTasksAsync(object parameter)
        {
            List<ItemGridSource> obj = (List<ItemGridSource>)parameter;

            var Changes = obj.Except(OriginalTaskObj).ToList();
            bool resp = await _taskService.SaveTasksItems(Changes, SelectedEmployee.EmployeeID);
            if (resp)
            {
                TasksSource = await _taskService.GetTasksAsync();
            }

        }

        private async void OnLoaded()
        {
            await Load();
        }

        private async Task Load()
        {

            employees = await this._employeeService.GetEmployees();
            NotifyPropertyChanged("employees");
            await WeekLoad();

        }
        private async Task WeekChangedAsync(object parm)
        {
            await LoadTimesheet();
        }
        private async Task WeekLoad()
        {
            WeeksSource = new Dictionary<string, int>();
            for (int i = 1; i < 53; i++)
            {
                var firstday = FirstDateOfWeek(DateTime.Today.Year, i);

                WeeksSource[firstday.Date.ToString()] = i;
            }
            SelectedWeek = 1 + DateTime.Now.DayOfYear / 7;
            await LoadTimesheet();
        }
        public async Task LoadTimesheet()
        {
            Originaltimesheets = await _taskService.GetTasksTimesheets(SelectedWeek);
            Cloneemployees = DeepCopy(employees);
            foreach (var item in Cloneemployees)
            {
                List<TimeSheet> Clone = DeepCopy(Originaltimesheets.Where(e => e.Employee.EmployeeID == item.EmployeeID)).ToList();
                if (Clone.Count != 0)
                {
                    foreach (var sheet in Clone)
                    {
                        int.TryParse(item.TWEHours, out int t);
                        item.TWEHours = (t + int.Parse(sheet.Hours)).ToString();
                    }
                    int.TryParse(item.TWEHours, out int a);
                    item.AWEHours = (a / Clone.Count).ToString();
                }


            }
            NotifyPropertyChanged("Cloneemployees");

        }
        public static DateTime FirstDateOfWeek(int year, int weekOfYear)
        {
            DateTime jan1 = new DateTime(year, 1, 1);
            int daysOffset = DayOfWeek.Thursday - jan1.DayOfWeek;
            DateTime firstThursday = jan1.AddDays(daysOffset);
            var cal = CultureInfo.CurrentCulture.Calendar;
            int firstWeek = cal.GetWeekOfYear(firstThursday, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
            var weekNum = weekOfYear;
            if (firstWeek == 1)
            {
                weekNum -= 1;
            }
            var result = firstThursday.AddDays(weekNum * 7);
            return result.AddDays(-5);
        }
        private async Task GetTasksByID(object parameter)
        {
            TasksSource = await _taskService.GetTasksAsync();
            TasksByIdSource = await _taskService.GetTasksAsyncByID(SelectedEmployee.EmployeeID, SelectedWeek);

            OriginalTaskObj = new List<ItemGridSource>();
            TotalValue = null;
            ItemGridSource Tvl = new ItemGridSource();
            var TotalValueToDic = Tvl.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).ToDictionary(prop => prop.Name, prop => prop.GetValue(Tvl, null));
            foreach (var item in TasksSource)
            {
                ItemGridSource s = new ItemGridSource();
                var ToDictionaryObj = s.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).ToDictionary(prop => prop.Name, prop => prop.GetValue(s, null));
                ToDictionaryObj["Id"] = item.TaskID;
                ToDictionaryObj["Name"] = item.Name;
                var toModelObj = GetObject<ItemGridSource>(ToDictionaryObj);
                OriginalTaskObj.Add(toModelObj);
            }
            foreach (var item in TasksByIdSource)
            {
                ItemGridSource s = new ItemGridSource();
                var ToDictionaryObj = s.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).ToDictionary(prop => prop.Name, prop => prop.GetValue(s, null));
                ToDictionaryObj["Id"] = item.TaskID;
                ToDictionaryObj["Name"] = item.Name;
                foreach (var val in item.TimeSheets)
                {
                    ToDictionaryObj[Enum.GetName(typeof(DayOfWeek), int.Parse(val.Day.Day))] = val.Hours;
                    var v = (string)TotalValueToDic[Enum.GetName(typeof(DayOfWeek), int.Parse(val.Day.Day))];
                    int.TryParse(v, out int i);
                    TotalValueToDic[Enum.GetName(typeof(DayOfWeek), int.Parse(val.Day.Day))] = (i + int.Parse(val.Hours)).ToString();
                    TotalValue = GetObject<ItemGridSource>(TotalValueToDic);
                }
                var toModelObj = GetObject<ItemGridSource>(ToDictionaryObj);
                OriginalTaskObj[OriginalTaskObj.FindIndex(i => i.Id == toModelObj.Id)] = toModelObj;

            }

            TaskObj = DeepCopy(OriginalTaskObj);
        }
        T GetObject<T>(Dictionary<string, object> dict)
        {
            Type type = typeof(T);
            var obj = Activator.CreateInstance(type);

            foreach (var kv in dict)
            {
                type.GetProperty(kv.Key).SetValue(obj, kv.Value);
            }
            return (T)obj;
        }
        public static T DeepCopy<T>(T source)
        {
            if (Object.ReferenceEquals(source, null))
            {
                return default(T);
            }
            var deserializeSettings = new JsonSerializerSettings { ObjectCreationHandling = ObjectCreationHandling.Replace };
            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(source), deserializeSettings);
        }
        private bool canExecute(object parameter)
        {
            return true;
        }


        public List<Employee> employees
        {
            get; set;
        }
        public List<Employee> Cloneemployees
        {
            get; set;
        }

        private List<data.Models.Task> _tasksSource;

        public List<data.Models.Task> TasksSource
        {
            get { return _tasksSource; }
            set
            {
                _tasksSource = value;
                NotifyPropertyChanged("TasksSource");
            }
        }
        private List<data.Models.Task> _tasksByIDSource;

        public List<data.Models.Task> TasksByIdSource
        {
            get { return _tasksByIDSource; }
            set
            {
                _tasksByIDSource = value;
                NotifyPropertyChanged("TasksByIdSource");
            }
        }
        private Employee _selectedEmployee;
        public Employee SelectedEmployee
        {
            get { return _selectedEmployee; }
            set
            {
                _selectedEmployee = value;
                NotifyPropertyChanged("SelectedEmployee");
            }
        }
        private List<ItemGridSource> _task;

        public List<ItemGridSource> TaskObj
        {
            get
            {
                return _task;
            }
            set
            {
                _task = value;
                NotifyPropertyChanged("TaskObj");
            }
        }

        private List<ItemGridSource> _originalTaskObj;

        public List<ItemGridSource> OriginalTaskObj
        {
            get
            {
                return _originalTaskObj;
            }
            set
            {
                _originalTaskObj = value;
                NotifyPropertyChanged("OriginalTaskObj");
            }
        }
        public List<DayOfWeek> _daysOfWeek = Enum.GetValues(typeof(DayOfWeek)).Cast<DayOfWeek>().ToList();
        public List<DayOfWeek> DaysOfWeek
        {
            get { return _daysOfWeek; }
            set
            {
                _daysOfWeek = value;
            }
        }
        private DayOfWeek _selectedDay;

        public DayOfWeek SelectedDay
        {
            get { return _selectedDay; }
            set
            {
                _selectedDay = value;
                NotifyPropertyChanged("SelectedDay");

            }
        }
        private data.Models.Task _selectedTask;

        public data.Models.Task SelectedTask
        {
            get { return _selectedTask; }
            set
            {
                _selectedTask = value;
                NotifyPropertyChanged("SelectedTask");

            }
        }
        private string _dayilyTotalHoures;

        public string DayilyTotalHoures
        {
            get { return _dayilyTotalHoures; }
            set
            {
                _dayilyTotalHoures = value;
                NotifyPropertyChanged("DayilyTotalHoures");
            }
        }

        private List<TimeSheet> _originaltimesheets;

        public List<TimeSheet> Originaltimesheets
        {
            get
            {
                return _originaltimesheets;
            }
            set
            {
                _originaltimesheets = value;
                NotifyPropertyChanged("Originaltimesheets");
            }
        }
        private TimeSheet timeSheets;

        public TimeSheet TimeSheets
        {
            get
            {
                return timeSheets;
            }
            set
            {
                timeSheets = value;
                NotifyPropertyChanged("TimeSheets");
            }
        }
        private data.Models.Task _taskByDay;

        public data.Models.Task TaskByDay
        {
            get
            {
                return _taskByDay;
            }
            set
            {
                _taskByDay = value;
                NotifyPropertyChanged("TaskByDay");
            }
        }
        private int _selectedWeek;
        public int SelectedWeek
        {
            get { return _selectedWeek; }
            set
            {
                _selectedWeek = value;
                NotifyPropertyChanged("SelectedWeek");
            }
        }

        private Dictionary<string, int> _weeksSource;

        public Dictionary<string, int> WeeksSource
        {
            get { return _weeksSource; }
            set
            {
                _weeksSource = value;
                NotifyPropertyChanged("WeeksSource");
            }
        }



        private ItemGridSource _totalValue;

        public ItemGridSource TotalValue
        {
            get { return _totalValue; }
            set
            {
                _totalValue = value;
                NotifyPropertyChanged("TotalValue");
            }
        }


    }
}
