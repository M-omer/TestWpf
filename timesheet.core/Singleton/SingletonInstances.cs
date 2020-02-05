using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace timesheet.core.Singleton
{
    public sealed class SingletonInstances
    {
        private static List<object> instances = new List<object>();
        public static object GetEmployeeService(Type T)
        {
            foreach(var obj in instances)
            {
                if (obj.GetType() == T) return obj;
            }         

            // create an object of the type
            var newobj = Activator.CreateInstance(T);
            instances.Add(newobj);
            return newobj;
        }
    }
}
