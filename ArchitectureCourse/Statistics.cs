using ArchitectureCourse.States;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace ArchitectureCourse
{
    public class Statistics
    {
        public double Lambda { get; private set; }
        public double Alpha { get; private set; }
        public double Beta { get; private set; }
        public int MaxBufferSize { get; private set; }
        public int RequestPerSource { get; private set; }

        public List<Source> Sources { get; private set; }
        public List<Device> Devices { get; private set; }

        public Analyzer Analyzer { get; private set; }
        public Buffer Buffer { get; private set; }
        public RequestGenerator RequestGenerator { get; private set; }
        public RequestHandler RequestHandler { get; private set; }

        public double SystemTime { get; private set; }

        public Statistics(double lambda, double alpha, double beta, int maxBufferSize, int requestPerSource, List<Source> sources, List<Device> devices)
        {
            Lambda = lambda;
            Alpha = alpha;
            Beta = beta;
            MaxBufferSize = maxBufferSize;
            RequestPerSource = requestPerSource;
            Sources = sources;
            Devices = devices;

            var sourceStates = new List<SourceState>();
            foreach (var source in Sources)
            {
                sourceStates.Add(source.SourceState);
            }

            var deviceStates = new List<DeviceState>();
            foreach (var device in Devices)
            {
                deviceStates.Add(device.DeviceState);
            }

            var bufferState = new BufferState()
            {
                Requests = new(),
                State = "Downtime"
            };

            Analyzer = new Analyzer(sourceStates, bufferState, deviceStates);

            Buffer = new Buffer(MaxBufferSize, Analyzer);

            RequestGenerator = new RequestGenerator(RequestPerSource, Sources, Analyzer, Buffer);

            var devicesSet = new SortedSet<Device>(devices);

            RequestHandler = new RequestHandler(devicesSet, Analyzer, Buffer);
        }

        public void Start()
        {
            var sw = Stopwatch.StartNew();

            while (!RequestGenerator.IsComplete() || !Buffer.IsEmpty)
            {
                RequestGenerator.Generate(sw.ElapsedTicks);
                RequestHandler.Handle(sw.ElapsedTicks);
            }

            SystemTime = (double)sw.ElapsedTicks / Stopwatch.Frequency;
        }

        public void AutoMode()
        {
            Console.WriteLine("****************** AUTO MODE ******************");
            Console.WriteLine("***********************************************");
            Console.WriteLine();
            Console.WriteLine("Sources:");
            Console.WriteLine($"{"ID",-4}|{"RequestAmount",-15}|{"RejectedAmount",-15}|{"RejectionProb",-20}|{"TimeInSystem",-15}|{"AvgRequestTime"}");
            Console.WriteLine("----------------------------------------------------------------------------------");
            Sources.Sort((x, y) => x.Id - y.Id);
            foreach (var source in Sources)
            {
                Console.WriteLine($"{source.Id,-4}|{source.RequestAmount,-15}|{source.RejectedAmount,-15}|{source.RejectionProb,-20:0.#####}|{source.TimeInSystem,-15:0.####}|{source.AvgRequestTime:0.####}");
            }

            Console.WriteLine();
            Console.WriteLine("Devices:");
            Console.WriteLine($"{"ID",-4}|{"Priority",-15}|{"Workload"}");
            Console.WriteLine("----------------------------------------------------------------------------------");
            foreach (var device in Devices)
            {
                var TimeInSystem = (device.TimeInSystem - device.LastInterval) / 100000 / SystemTime;
                Console.WriteLine($"{device.Id,-4}|{device.Priority,-15:0.####}|{TimeInSystem}");
            }

            Console.WriteLine();
            Console.WriteLine("***********************************************");
        }

        public void StepMode()
        {
            var events = Analyzer.getEvents();
            Console.WriteLine("****************** STEP MODE ******************");

            for (int i = 0; i < events.Count; i++)
            {
                var e = events[i];
                Console.WriteLine("***********************************************");
                Console.WriteLine();
                Console.WriteLine($"Step: {i}, Event: {e.Event}, Time: {e.Time}");
                Console.WriteLine();

                Console.WriteLine("Sources:");
                Console.WriteLine($"{"ID",-4}|{"Time",-15}|{"RequestAmount",-15}|{"RefectedAmount",-15}|{"State"}");
                Console.WriteLine("----------------------------------------------------------------------------------");
                foreach (var ss in e.SourceStates)
                {
                    Console.WriteLine($"{ss.SourceId,-4}|{ss.Time,-15:0.###}|{ss.RequestAmount,-15}|{ss.RefectedAmount,-15}|{ss.State}");
                }
                Console.WriteLine();

                Console.WriteLine($"Buffer[size:{e.BufferState.Requests.Count}]: {e.BufferState.State}");
                Console.WriteLine($"{"N",-4}|{"SourceId",-15}|{"RequestId",-15}");
                Console.WriteLine("----------------------------------------------------------------------------------");
                for (int j = 0; j < e.BufferState.Requests.Count; j++)
                {
                    var bb = e.BufferState.Requests[j];
                    Console.WriteLine($"{j,-4}|{bb.SourceId,-15}|{bb.Id,-15}");
                }
                Console.WriteLine();

                Console.WriteLine("Devices:");
                Console.WriteLine($"{"ID",-4}|{"Priority",-15}|{"Time",-15}|{"State"}");
                Console.WriteLine("----------------------------------------------------------------------------------");
                foreach (var dd in e.DeviceStates)
                {
                    Console.WriteLine($"{dd.DeviceId,-4}|{dd.DevicePriority,-15}|{dd.Time,-15:0.###}|{dd.State}");
                }
                Console.WriteLine();
                Console.WriteLine("***********************************************");
                Console.ReadKey();
            }
        }
    }
}
