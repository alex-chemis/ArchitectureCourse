using ArchitectureCourse;
using ArchitectureCourse.States;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArchitectureCourse
{
    public class Source
    {
        static private int ID = 0;

        private double _time;

        public double TimeInSystem => _time;
        public int RejectedAmount { get; set; }
        public int RequestAmount { get; set; }

        public double RejectionProb { get => (double)RejectedAmount / RequestAmount; }

        public double AvgRequestTime { get => TimeInSystem / RequestAmount; }

        public int Id { get; init; }

        public double Lambda { get; init; }

        private Random Random { get; init; } = new Random();

        public SourceState SourceState { get; init; }

        public Source(double lambda)
        {
            Id = ID++;
            Lambda = lambda;
            SourceState = new SourceState()
            {
                SourceId = Id,
                Time = 0,
                RequestAmount = 0,
                RefectedAmount = 0,
                State = "Downtime",
            };
            _time = GetTimePoint();
        }

        private double GetInterval()
        {
            return -1.0 / Lambda * Math.Log(Random.NextDouble());
        }

        public Request Generate()
        {
            var interval = GetInterval();
            _time += interval;
            SourceState.Time = interval;
            SourceState.RequestAmount++;
            var ret = new Request(Id);
            SourceState.State = $"Generate request id:{ret.Id}";
            return ret;
        }

        public double GetTimePoint()
        {
            return _time / 100000;
        }
    }
}
