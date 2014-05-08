	// -----------------------------------------------------------------------
// <copyright file="GetCounterCommand.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace PerfTap.Counter
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading;
	using PerfTap.Interop;
    using PerfTap.Configuration;

	public class PerfmonCounterReader
	{		
		private readonly IEnumerable<string> _computerNames = new string[0];
		private readonly bool _ignoreBadStatusCodes = true; 
		private const int INFINITIY = -1;

		public PerfmonCounterReader(IEnumerable<string> computerNames)
		{			
			if (null == computerNames) { throw new ArgumentNullException("computerNames"); }

			this._computerNames = computerNames;
		}
		
		public PerfmonCounterReader()
		{
			this._computerNames = new string[0];
		}

		public PerfmonCounterReader(bool ignoreBadStatusCodes)
		{
			this._computerNames = new string[0];
			this._ignoreBadStatusCodes = ignoreBadStatusCodes;
		}

        public IEnumerable<PerformanceCounterSampleSet> GetCounterSamples(TimeSpan sampleInterval, int count, CancellationToken token)
        {
            if (count <= 0) { throw new ArgumentOutOfRangeException("count", "must be greater than zero"); }
            if (null == token) { throw new ArgumentNullException("token"); }

            return ProcessGetCounter(GetDefaultCounters(), sampleInterval, count, token);
        }

        public IEnumerable<PerformanceCounterSampleSet> StreamCounterSamples(TimeSpan sampleInterval, CancellationToken token)
        {
            if (null == token) { throw new ArgumentNullException("token"); }

            return ProcessGetCounter(GetDefaultCounters(), sampleInterval, INFINITIY, token);
        }

        public IEnumerable<PerformanceCounterSampleSet> GetCounterSamples(List<CounterName> counters, TimeSpan sampleInterval, int count, CancellationToken token)
		{
			if (null == counters) { throw new ArgumentNullException("counters"); }
			if (count <= 0) { throw new ArgumentOutOfRangeException("count", "must be greater than zero"); }
			if (null == token) { throw new ArgumentNullException("token"); }

			return ProcessGetCounter(counters, sampleInterval, count, token);
		}

        public IEnumerable<PerformanceCounterSampleSet> StreamCounterSamples(List<CounterName> counters, TimeSpan sampleInterval, CancellationToken token)
		{
			if (null == counters) { throw new ArgumentNullException("counters"); }
			if (null == token) { throw new ArgumentNullException("token"); }

			return ProcessGetCounter(counters, sampleInterval, INFINITIY, token);
		}

        private IEnumerable<PerformanceCounterSampleSet> ProcessGetCounter(List<CounterName> counters, TimeSpan sampleInterval, int maxSamples, CancellationToken token)
		{
			using (PdhHelper helper = new PdhHelper(counters, this._ignoreBadStatusCodes))
			{
				int samplesRead = 0;

				do
				{
					PerformanceCounterSampleSet set = helper.ReadNextSet();
					if (null != set)
					{
						yield return set;
					}
					samplesRead++;
				}
				while (((maxSamples == INFINITIY) || (samplesRead < maxSamples)) && !token.WaitHandle.WaitOne(sampleInterval, true));
			}
		}

        public static List<CounterName> DefaultCounters
		{
			get 
			{ 
				return new List<CounterName> {
                    new CounterName { Name = @"@\network interface(*)\bytes total/sec", MetricSuffix = "net.{0}.bytes_total_sec"},
                    new CounterName { Name = @"\processor(_total)\% processor time", MetricSuffix = "cpu.processor_time"},
                    };

                    //new List<string>() { 
                    //    @"\network interface(*)\bytes total/sec|net.{0}.bytes_total_sec", 
                    //    @"\processor(_total)\% processor time|cpu.processor_time", 
                    //    @"\memory\% committed bytes in use|memory.%_comitted", 
                    //    @"\memory\cache faults/sec|memory.cache_faults_sec", 
                    //    @"\physicaldisk(_total)\% disk time|disk.%_time", 
                    //    @"\physicaldisk(_total)\current disk queue length|disk.current_queue" 
                    //});
			}
		}

        private List<CounterName> GetDefaultCounters()
        {
            return DefaultCounters;
        }
	}
}