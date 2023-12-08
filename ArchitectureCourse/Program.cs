using ArchitectureCourse;
using ArchitectureCourse.States;
using System.Diagnostics;

double lambda = 10;
double alpha = 2;
double beta = 500;
int maxBufferSize = 6000;
int requestPerSource = 1000;

var sources = new List<Source> { new Source(lambda), new Source(lambda), new Source(lambda), new Source(lambda), new Source(lambda), new Source(lambda), new Source(lambda) };
var devices = new List<Device> { new Device(1, alpha, beta), new Device(2, alpha, beta), new Device(3, alpha, beta), new Device(4, alpha, beta), new Device(5, alpha, beta), new Device(6, alpha, beta) };

var statistics = new Statistics(lambda, alpha, beta, maxBufferSize, requestPerSource, sources, devices);

statistics.Start();
statistics.AutoMode();
statistics.StepMode();
