using ArchitectureCourse.States;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArchitectureCourse
{
    public class RequestEvent
    {
        public string Event { get; set; } = null!;
        public Request Request { get; set; } = null!;
        public double Time { get; set; }
        public List<SourceState> SourceStates { get; set; } = null!;
        public BufferState BufferState { get; set; } = null!;
        public List<DeviceState> DeviceStates { get; set; } = null!;
    }
}
