using System.Diagnostics.Metrics;

using JGUZDV.Extensions.OpenTelemetry;

using Microsoft.Extensions.Options;

namespace JGUZDV.JobHost.Sample
{
    /// <summary>
    /// A sample meter container required for reporting of metrics.
    /// </summary>
    public class SampleMeterContainer : JGUZDVBaseMeter
    {
        private readonly Counter<int> _sampleCounter;

        /// <summary>
        /// Instantiates the container, meter and a sample counter.
        /// </summary>
        /// <param name="options"></param>
        public SampleMeterContainer(IOptions<JGUZDVOpenTelemetryOptions> options) :base(options) 
        {
            _sampleCounter = Meter.CreateCounter<int>(
                name: "SampleCounter", 
                description: "A sample counter to transmit metrics");
        }

        /// <summary>
        /// Used to increment the sample counter.
        /// </summary>
        /// <param name="amount"></param>
        public void IncrementSampleCounter(int amount = 1)
        {
            _sampleCounter.Add(amount);
        }
    }
}
