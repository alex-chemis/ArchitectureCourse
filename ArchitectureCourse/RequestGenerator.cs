using ArchitectureCourse;
using Buffer = ArchitectureCourse.Buffer;
using System.CodeDom.Compiler;
using System.Diagnostics;

namespace ArchitectureCourse
{
    public class RequestGenerator
    {
        private readonly List<Source> _sources;
        private readonly int _requestPerSource;
        private int _requestGenerated;
        private int _actual_source;
        public Analyzer Analyzer { get; set; }
        public Buffer Buffer { get; set; }

        private Random Random { get; init; } = new Random();

        public RequestGenerator(int requestPerSource, List<Source> sources, Analyzer analyzer, Buffer buffer)
        {
            _sources = sources;
            _requestPerSource = requestPerSource;
            _actual_source = 0;
            Analyzer = analyzer;
            Buffer = buffer;
        }

        public void Generate(long elapsedTicks)
        {
            var source = _sources[_actual_source];

            if (elapsedTicks > Math.Round(source.GetTimePoint() * Stopwatch.Frequency) && source.RequestAmount < _requestPerSource)
            {
                var request = source.Generate();
                Analyzer.AddEvent(new RequestEvent()
                {
                    Request = request,
                    Time = (double)elapsedTicks / Stopwatch.Frequency,
                    Event = $"Source[id:{source.Id}] generate request[id:{request.Id}]"
                });
                source.SourceState.Time = 0;
                source.SourceState.State = "Downtime";
                if (Buffer.Add(request, elapsedTicks))
                {
                    source.RejectedAmount++;
                }
                source.RequestAmount++;
                _requestGenerated++;
            }

            _actual_source = (_actual_source + 1) % _sources.Count;

            //for (int i = _sources.Count - 1; i >= 1; i--)
            //{
            //    int j = Random.Next(i + 1);
            //    // обменять значения data[j] и data[i]
            //    var temp = _sources[j];
            //    _sources[j] = _sources[i];
            //    _sources[i] = temp;
            //}
            //foreach (var source in _sources)
            //{
            //    if (elapsedTicks > Math.Round(source.GetTimePoint() * Stopwatch.Frequency) && source.RequestAmount < _requestPerSource)
            //    {
            //        var request = source.Generate();
            //        Analyzer.AddEvent(new RequestEvent()
            //        {
            //            Request = request,
            //            Time = (double)elapsedTicks / Stopwatch.Frequency,
            //            Event = $"Source[id:{source.Id}] generate request[id:{request.Id}]"
            //        });
            //        source.SourceState.Time = 0;
            //        source.SourceState.State = "Downtime";
            //        if (Buffer.Add(request, elapsedTicks))
            //        {
            //            source.RejectedAmount++;
            //        }
            //        source.RequestAmount++;
            //        _requestGenerated++;
            //        break;
            //    }
            //}
        }

        public bool IsComplete()
        {
            return _requestGenerated == (_requestPerSource * _sources.Count);
        }
    }
}
