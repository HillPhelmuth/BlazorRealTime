using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorRealTime.Shared
{
    public class ScreenDataItem
    {
        public int Id { get; set; }
        public string Value { get; set; }
       
        public override string ToString()
        {
            return Value;
        }

        //public static implicit operator string(ScreenDataItem v)
        //{
        //    return v.Value;
        //}
    }
}
