using ArchitectureCourse.States;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace ArchitectureCourse
{
    public class Buffer
    {
        private readonly List<Request> _requests;
        private readonly int _maxSize;

        public Analyzer Analyzer { get; set; }

        public bool IsEmpty { get => _requests.Count == 0; }

        public Buffer(int maxSize, Analyzer analyzer)
        {
            _requests = new List<Request>();
            _maxSize = maxSize;
            Analyzer = analyzer;
        }

        public bool Add(Request request, long elapsedTicks)
        {
            bool IsRejected = false;
            if (_requests.Count == _maxSize)
            {
                var ret = _requests.ElementAt(0);
                _requests.RemoveAt(0);
                IsRejected = true;
                Analyzer.BufferState.State = $"Request[id:{ret.Id}, source_id:{ret.SourceId}] has been rejected";
            }
            _requests.Add(request);
            Analyzer.BufferState.Requests = _requests.ToList();
            Analyzer.AddEvent(new RequestEvent()
            {
                Request = request,
                Time = (double)elapsedTicks / Stopwatch.Frequency,
                Event = $"Request[id:{request.Id}, source_id:{request.SourceId}] has been added to the buffer"
            });
            Analyzer.BufferState.State = "";
            return IsRejected;
        }

        public Request? Get(long elapsedTicks)
        {
            var ret = _requests.ElementAtOrDefault(_requests.Count - 1);
            if (ret != null)
            {
                Analyzer.AddEvent(new RequestEvent()
                {
                    Request = ret,
                    Time = (double)elapsedTicks / Stopwatch.Frequency,
                    Event = $"Request[id:{ret.Id}, source_id:{ret.SourceId}] has been send to the device"
                });
                _requests.RemoveAt(_requests.Count - 1);
            }
            return ret;
        }
    }
}
