using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorGrpcWebApp.Shared
{
    public class UserUnit
    {
        public int UserId { get; set; }
        public int UnitId { get; set; }
        public int HitPoints { get; set; }
    }
}
