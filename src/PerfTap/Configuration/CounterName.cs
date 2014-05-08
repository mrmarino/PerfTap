namespace PerfTap.Configuration
{
	using System;
	using System.Configuration;

	public class CounterName : ConfigurationElement, ICounterName
	{
		[ConfigurationProperty("name", IsRequired = true)]
		public string Name
		{
			get { return (string)this["name"]; }
			set { this["name"] = value; }
		}

        [ConfigurationProperty("metricSuffix", IsRequired = true)]
        public string MetricSuffix
        {
            get { return (string)this["metricSuffix"]; }
            set { this["metricSuffix"] = value; }
        }
	}
}