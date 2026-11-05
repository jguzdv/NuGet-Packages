using System.Diagnostics.Metrics;

using JGUZDV.Extensions.OpenTelemetry;

using Microsoft.Extensions.Options;

namespace JGUZDV.JobHost.Sample
{
    /// <summary>
    /// A sample meter container required for reporting of metrics.
    /// </summary>
    public class SampleMeterContainer
    {
        private readonly Counter<int> _sampleCounter;
        private readonly Meter _meter;

        /// <summary>
        /// Instantiates the container, meter and a sample counter.
        /// </summary>
        public SampleMeterContainer(Meter meter)
        {
            _meter = meter;
            _sampleCounter = meter.CreateCounter<int>(
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
