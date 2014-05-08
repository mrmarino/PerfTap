// -----------------------------------------------------------------------
// <copyright file="PerformanceCounterSamplesExtensionsTests.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace PerfTap.Counter.Tests
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Linq;
	using System.Text;
	using NanoTube.Support;
	using PerfTap;
	using Xunit;
	using Xunit.Extensions;

	/// <summary>
	/// TODO: Update summary.
	/// </summary>
	public class PerformanceCounterSamplesExtensionsTests
	{
		[Theory]
		[InlineData("ju!nk")]
		[InlineData("^")]
		[InlineData("#")]
		[InlineData("$")]
		[InlineData(" foo")]
		[InlineData(";")]
		[InlineData("test:key")]
		[InlineData("key/check")]
		[InlineData("my.key")]
		[InlineData("my(key)")]
		[InlineData("this\\key\\happy")]
		[InlineData("%")]
		public void ToGraphiteString_ThrowsOnInvalidKey(string key)
		{
			var samples = new [] { new PerformanceCounterSample(@"\\machine-names\memory\% committed bytes in use", null, 36.41245914, 818220, 2247088, 1, PerformanceCounterType.RawFraction, 0, 3579545, DateTime.Now, (ulong)DateTime.Now.ToFileTime(), 0, "test.memory.commited") };
			Assert.Throws<ArgumentException>(() => samples.ToStatsiteString(key).ToList());
		}

		public static IEnumerable<object[]> ExpectedMetricConversions
		{
			get
			{
				DateTime now = DateTime.Now;
				yield return new object[] { 
					"key",
					new PerformanceCounterSample(@"\\machine-names\memory\% committed bytes in use", null, 36.41245914, 818220, 2247088, 1, PerformanceCounterType.RawFraction, 0, 3579545, now, (ulong)now.ToFileTime(), 0, "test.memory.commited"), 
					String.Format(@"key.machine-names.memory.pct_committed_bytes_in_use:36.412|kv|@{0}", now.AsUnixTime()) };

				yield return new object[] { 
					null,
					new PerformanceCounterSample(@"\\machine-names\processor(0)\% processor time", "0", 1.6925116, 3559922343750, 129708662189268994, 1, PerformanceCounterType.Timer100NsInverse, 0, 10000000, now, (ulong)now.ToFileTime(), 0, "test.cpu.processor_time"), 
					String.Format(@"machine-names.processor.0.pct_processor_time:1.693|kv|@{0}", now.AsUnixTime()) };

				yield return new object[] { 
					"foo",
					new PerformanceCounterSample(@"\\machine-names\processor(_total)\% processor time", "_total", 1.6925116, 3559922343750, 129708662189268994, 1, PerformanceCounterType.Timer100NsInverse, 0, 10000000, now, (ulong)now.ToFileTime(), 0, "test.cpu.processor_time"), 
					String.Format(@"foo.machine-names.processor._total.pct_processor_time:1.693|kv|@{0}", now.AsUnixTime()) };

				yield return new object[] { 
					"junk",
					new PerformanceCounterSample(@"\\machine-names\System\Context Switches/sec", null, 3019.49343554114, 155783595, 149122840759, 1, PerformanceCounterType.RateOfCountsPerSecond32, 4294967294, 3579545, now, (ulong)now.ToFileTime(), 0, "test.system.context_switches"), 
					String.Format(@"junk.machine-names.system.context_switches_sec:3019.493|kv|@{0}", now.AsUnixTime()) };

				yield return new object[] { 
					"super-key",
					new PerformanceCounterSample(@"\\machine-names\physicaldisk(_total)\avg. disk sec/write", "_total", 0.000599983144971405, 840816148, 181869, 1, PerformanceCounterType.AverageTimer32, 3, 3579545, now, (ulong)now.ToFileTime(), 0, "test.disk._total.avg_disk_write"), 
					String.Format(@"super-key.machine-names.physicaldisk._total.avg__disk_sec_write:0.001|ms") };

				yield return new object[] {
					"foo",
					new PerformanceCounterSample(@"\\machine-names\network interface(isatap.localdomain)\bytes total/sec", "isatap.localdomain", 0, 0, 345497015725, 1, PerformanceCounterType.RateOfCountsPerSecond64, 4294967292, 3579545, now, (ulong)now.ToFileTime(), 0, "test.net.localdomain.bytes_total"),
					String.Format(@"foo.machine-names.network_interface.isatap_localdomain.bytes_total_sec:0|kv|@{0}", now.AsUnixTime()) };
			}
		}

		[Theory]
		[PropertyData("ExpectedMetricConversions")]
		public void ToGraphiteString_GeneratesExpectedMetrics(string key, PerformanceCounterSample sample, string expected)
		{			
			string converted = new [] { sample }.ToStatsiteString(key).First();
			Assert.Equal(expected, converted);
		}
	}
}
