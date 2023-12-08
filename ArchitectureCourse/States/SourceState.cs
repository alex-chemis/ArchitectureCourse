using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArchitectureCourse.States
{
    public class SourceState
    {
        public int SourceId { get; set; }
        public double Time { get; set; }
        public int RequestAmount { get; set; }
        public int RefectedAmount { get; set; }
        public string State { get; set; } = null!;
    }
}
