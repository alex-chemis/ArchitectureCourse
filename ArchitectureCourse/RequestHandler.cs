using ArchitectureCourse;
using System;
using System.Diagnostics;
using Buffer = ArchitectureCourse.Buffer;

namespace ArchitectureCourse
{
    public class RequestHandler
    {
        public class DevicesByPriority
        {
            public List<Device> Devices { get; set; } = null!;
            public int ActualDevice { get; set; }
        }

        private SortedSet<Device> _devices;

        private SortedDictionary<int, DevicesByPriority> _devicesByPriority;

        public Analyzer Analyzer { get; set; }
        public Buffer Buffer { get; set; }

        public RequestHandler(SortedSet<Device> devices, Analyzer analyzer, Buffer buffer)
        {
            _devices = devices;
            _devicesByPriority = new SortedDictionary<int, DevicesByPriority>();
            foreach (var device in _devices)
            {
                _devicesByPriority.TryAdd(device.Priority, new DevicesByPriority()
                {
                    Devices = new List<Device>(),
                    ActualDevice = 0
                });
            }
            foreach (var device in _devices)
            {
                _devicesByPriority[device.Priority].Devices.Add(device);
            }
            Analyzer = analyzer;
            Buffer = buffer;
        }

        public void Handle(long elapsedTicks)
        {
            foreach (var devices in _devicesByPriority.Reverse())
            {
                var device = devices.Value.Devices[devices.Value.ActualDevice];
                devices.Value.ActualDevice = (devices.Value.ActualDevice + 1) % devices.Value.Devices.Count;
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

            //foreach (var device in _devices)
            //{
            //    if (elapsedTicks > Math.Round(device.GetTimePoint() * Stopwatch.Frequency))
            //    {
            //        var ret = Buffer.Get(elapsedTicks);
            //        if (ret != null)
            //        {
            //            device.Handle(ret);
            //            Analyzer.AddEvent(new RequestEvent()
            //            {
            //                Request = ret,
            //                Time = (double)elapsedTicks / Stopwatch.Frequency,
            //                Event = $"Request[id:{ret.Id}, source_id:{ret.SourceId}] has been started process by device[id:{device.Id}]"
            //            });
            //            break;
            //        }
            //    }
            //    else
            //    {
            //        device.DeviceState.State = "Busy";
            //    }
            //}
        }
    }
}
