using ArchitectureCourse;
using System;
using System.Diagnostics;
using Buffer = ArchitectureCourse.Buffer;

namespace ArchitectureCourse
{
    public class RequestHandler
    {
        private readonly SortedSet<Device> _devices;

        public Analyzer Analyzer { get; set; }
        public Buffer Buffer { get; set; }

        

        public RequestHandler(SortedSet<Device> devices, Analyzer analyzer, Buffer buffer)
        {
            _devices = devices;
            Analyzer = analyzer;
            Buffer = buffer;
        }

        public void Handle(long elapsedTicks)
        {
            
            foreach (var device in _devices)
            {
                if (elapsedTicks > Math.Round(device.GetTimePoint() * Stopwatch.Frequency))
                {
                    var ret = Buffer.Get(elapsedTicks);
                    if (ret != null)
                    {
                        device.Handle(ret);
                        Analyzer.AddEvent(new RequestEvent()
                        {
                            Request = ret,
                            Time = (double)elapsedTicks / Stopwatch.Frequency,
                            Event = $"Request[id:{ret.Id}, source_id:{ret.SourceId}] has been started process by device[id:{device.Id}]"
                        });
                        break;
                    }
                    
                }
                else
                {
                    device.DeviceState.State = "Busy";
                }
            }
        }
    }
}
