namespace PerfTap.Configuration
{
	using System;
	using System.Configuration;
	using System.Collections.Generic;

	public class CounterFilterConfigurationCollection : ConfigurationElementCollection
	{
		/// <summary>
		/// Initializes a new instance of the CounterNameConfigurationCollection class.
		/// </summary>
		public CounterFilterConfigurationCollection()
		{ }

        public CounterFilterConfigurationCollection(IEnumerable<string> expressions)
		{
			foreach (var exp in expressions)
			{
				this.BaseAdd(new CounterFilter() { Expression = exp });
			}			
		}

		protected override ConfigurationElement CreateNewElement()
		{
			return new CounterFilter();
		}

		protected override object GetElementKey(ConfigurationElement element)
		{
			return ((CounterFilter)element).Expression;
		}
	}
}