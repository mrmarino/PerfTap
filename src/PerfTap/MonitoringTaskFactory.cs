// -----------------------------------------------------------------------
// <copyright file="MonitoringTaskFactory.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace PerfTap
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using PerfTap.Configuration;
    using PerfTap.Counter;
    using Graphite;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class MonitoringTaskFactory
    {
        private readonly ICounterSamplingConfiguration _counterSamplingConfig;
        private readonly List<CounterName> _counterPaths;
        private readonly CounterNameConfigurationCollection _counterDefinitions;
        private readonly List<CounterFilter> _counterFilters;
        /// <summary>
        /// Initializes a new instance of the MonitoringTaskFactory class.
        /// </summary>
        /// <param name="counterSamplingConfig"></param>
        public MonitoringTaskFactory(ICounterSamplingConfiguration counterSamplingConfig)
        {
            if (null == counterSamplingConfig) { throw new ArgumentNullException("counterSamplingConfig"); }

            _counterSamplingConfig = counterSamplingConfig;
            
            _counterFilters = counterSamplingConfig.Filters.Select(x => new CounterFilter { Expression = x.Expression}).ToList();
            _counterPaths = counterSamplingConfig.DefinitionFilePaths
                .SelectMany(path => CounterFileParser.ReadCountersFromFile(path.Path)).Select(item => new CounterName
                {
                    Name = item.Split('|')[0],
                    MetricSuffix = item.Split('|')[1]
                }).ToList();
        }

        public Task CreateContinuousTask(CancellationToken cancellationToken)
        {
            return new Task(() =>
            {
                var reader = new PerfmonCounterReader();

                using (GraphiteUdpClient client = new GraphiteUdpClient(_counterSamplingConfig.MetricHost, _counterSamplingConfig.MetricHostPort, _counterSamplingConfig.MetricPrefix))
                {
                    foreach (var samples in reader.StreamCounterSamples(_counterPaths, _counterSamplingConfig.SampleInterval, cancellationToken))
                    {
                        foreach (PerformanceCounterSample metric in samples.CounterSamples)
                        {
                            foreach (CounterFilter filter in _counterFilters)
                            {
                                if (!metric.IsFiltered(_counterFilters))
                                {
                                    client.Send(metric.MetricPath, metric.MetricValue, metric.Timestamp);
                                }
                            }
                        }
                    }
                }
            }, cancellationToken);
        }

        //public Task CreateTask(CancellationToken cancellationToken, int maximumSamples)
        //{
        //    return new Task(() =>
        //        {
        //            var reader = new PerfmonCounterReader();

        //            using (var messenger = new UdpMessenger(_metricPublishingConfig.HostName, _metricPublishingConfig.Port))
        //            {
        //                foreach (var metricBatch in reader.GetCounterSamples(_counterPaths, _counterSamplingConfig.SampleInterval, maximumSamples, cancellationToken)
        //                    .SelectMany(set => set.CounterSamples.ToStatsiteString(_metricPublishingConfig.PrefixKey))
        //                    .Chunk(10))
        //                {
        //                    messenger.SendMetrics(metricBatch);
        //                }
        //            }
        //        }, cancellationToken);
        //}
    }
}