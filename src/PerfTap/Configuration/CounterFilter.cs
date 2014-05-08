namespace PerfTap.Configuration
{
	using System;
	using System.Configuration;

    public class CounterFilter : ConfigurationElement, ICounterFilter
    {
        [ConfigurationProperty("expression", IsRequired = true)]
        public string Expression
        {
            get { return (string)this["expression"]; }
            set { this["expression"] = value; }
        }
    }
}