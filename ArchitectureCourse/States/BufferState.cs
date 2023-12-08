using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArchitectureCourse.States
{
    public class BufferState
    {
        public List<Request> Requests { get; set; } = null!;
        public string State { get; set; } = null!;
    }
}
