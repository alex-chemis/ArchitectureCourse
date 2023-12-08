using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArchitectureCourse.States
{
    public class DeviceState
    {
        public int DeviceId { get; set; }
        public int DevicePriority { get; set; }
        public double Time { get; set; }
        public string State { get; set; } = null!;
    }
}
