namespace PerfTap.Configuration
{
	using System;
	using System.Configuration;
	using System.Collections.Generic;

	public class CounterNameConfigurationCollection : ConfigurationElementCollection
	{
		/// <summary>
		/// Initializes a new instance of the CounterNameConfigurationCollection class.
		/// </summary>
		public CounterNameConfigurationCollection()
		{ }

		public CounterNameConfigurationCollection(IEnumerable<string> names)
		{
			foreach (string name in names)
			{
                var counter = name.Split('|');

                this.BaseAdd(new CounterName() { 
                    Name = counter[0],
                    MetricSuffix = counter[1]
                });
			}			
		}

		protected override ConfigurationElement CreateNewElement()
		{
			return new CounterName();
		}

		protected override object GetElementKey(ConfigurationElement element)
		{
			return ((CounterName)element).Name;
		}
	}
}