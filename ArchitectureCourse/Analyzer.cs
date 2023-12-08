using ArchitectureCourse.States;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArchitectureCourse
{
    public class Analyzer
    {
        private readonly List<RequestEvent> _events = new List<RequestEvent>();
        public List<SourceState> SourceStates { get; private init; }
        public BufferState BufferState { get; private init; }
        public List<DeviceState> DeviceStates { get; private init; }

        public Analyzer(List<SourceState> sourceStates, BufferState bufferState, List<DeviceState> deviceStates)
        {
            SourceStates = sourceStates;
            BufferState = bufferState;
            DeviceStates = deviceStates;
        }

        public void AddEvent(RequestEvent requestEvent)
        {
            lock (this)
            {
                requestEvent.BufferState = new()
                {
                    Requests = BufferState.Requests.ToList(),
                    State = BufferState.State
                };

                requestEvent.SourceStates = new();
                foreach (var state in SourceStates)
                {
                    requestEvent.SourceStates.Add(new()
                    {
                        SourceId = state.SourceId,
                        State = state.State,
                        RefectedAmount = state.RefectedAmount,
                        RequestAmount = state.RequestAmount,
                        Time = state.Time
                    });
                }

                requestEvent.DeviceStates = new();
                foreach (var state in DeviceStates)
                {
                    requestEvent.DeviceStates.Add(new()
                    {
                        DeviceId = state.DeviceId,
                        DevicePriority = state.DevicePriority,
                        Time = state.Time,
                        State = state.State
                    });
                }
            }

            _events.Add(requestEvent);
        }

        public List<RequestEvent> getEvents()
        {
            return _events;
        }
    }
}
