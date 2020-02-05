using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace timesheet.data.Models
{

    public class ItemGridSource
    {

        public int Id { get; set; }
        public string Name { get; set; }

        public string Saturday { get; set; }
        public string Sunday { get; set; }
        public string Monday { get; set; }
        public string Tuesday { get; set; }
        public string Wednesday { get; set; }
        public string Thursday { get; set; }
        public string Friday { get; set; }

        public override bool Equals(object obj)
        {
            var newObj = obj as ItemGridSource;

            if (null != newObj)
            {
                return this.GetHashCode() == newObj.GetHashCode();
            }
            else
            {
                return base.Equals(obj);
            }
        }
        public override int GetHashCode()
        {
            unchecked 
            {
                int hash = 17;
                
                hash = hash * 23 + Id.GetHashCode();
                hash = hash * 23 + (Name ?? "").GetHashCode();
                hash = hash * 23 + (Saturday ?? "").GetHashCode();
                hash = hash * 23 + (Sunday ?? "").GetHashCode();
                hash = hash * 23 + (Monday ?? "").GetHashCode();
                hash = hash * 23 + (Tuesday ?? "").GetHashCode();
                hash = hash * 23 + (Wednesday ?? "").GetHashCode();
                hash = hash * 23 + (Thursday ?? "").GetHashCode();
                hash = hash * 23 + (Friday ?? "").GetHashCode();
                return hash;
            }
        }
    }
    
    

}
