namespace PerfTap.Configuration
{
	using System;

	public interface ICounterName
	{
		string Name { get; }
        string MetricSuffix { get; }
	}
}