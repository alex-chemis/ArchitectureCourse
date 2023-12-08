using ArchitectureCourse.States;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArchitectureCourse
{
    public class Device: IComparable<Device>
    {
        static private int ID = 0;

        private double _time;
        public double LastInterval { get; private set; }

        public double TimeInSystem => _time;

        public int Id { get; init; }
        public int Priority { get; init; }
        public double Alpha { get; init; }
        public double Beta { get; init; }

        private Random Random { get; init; } = new Random();

        public DeviceState DeviceState { get; init; }

        public Device(int priority, double alpha, double beta)
        {
            Id = ID++;
            Priority = priority;
            Alpha = alpha;
            Beta = beta;

            DeviceState = new DeviceState()
            {
                DeviceId = Id,
                DevicePriority = Priority,
                Time = 0,
                State = "Downtime"
            };
        }

        public double GetInterval()
        {
            return Random.NextDouble() * (Beta - Alpha) + Alpha;
        }

        public void Handle(Request request)
        {
            LastInterval = GetInterval();
            _time += LastInterval;
            DeviceState.Time = LastInterval;
            DeviceState.State = $"Hanlde request id:{request.Id}, source_id:{request.SourceId}";
        }

        public double GetTimePoint()
        {
            return _time / 100000;
        }

        public int CompareTo(Device? other)
        { 
            if (other == null) throw new NullReferenceException();
            int byPriority = other.Priority - Priority;
            return byPriority == 0 ? Id - other.Id : byPriority;
        }
    }
}
