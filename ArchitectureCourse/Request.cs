using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArchitectureCourse
{
    public class Request
    {
        static private int ID = 0;
        static private object lockObject = new object();
        public int Id { get; init; }
        public int SourceId { get; init; }

        public Request(int sourceId)
        {
            lock (lockObject)
            {
                Id = ID;
                ID++;
                SourceId = sourceId;
            }
        }
    }
}
