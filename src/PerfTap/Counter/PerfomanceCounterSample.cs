// -----------------------------------------------------------------------
// <copyright file="PerfomanceCounterSample.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace PerfTap.Counter
{
    using PerfTap.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

    public class PerformanceCounterSample
    {
        public PerformanceCounterSample(string path, string instanceName, double cookedValue, ulong rawValue, ulong secondValue, uint multiCount, PerformanceCounterType counterType, uint defaultScale, ulong timeBase, DateTime timeStamp, ulong timeStamp100nSec, uint status, string metricSuffix)
        {
            this.Path = path;
            this.InstanceName = instanceName;
            this.CookedValue = cookedValue;
            this.RawValue = rawValue;
            this.SecondValue = secondValue;
            this.MultipleCount = multiCount;
            this.CounterType = counterType;
            this.DefaultScale = defaultScale;
            this.TimeBase = timeBase;
            this.Timestamp = timeStamp;
            this.Timestamp100NSec = timeStamp100nSec;
            this.Status = status;
            this.MetricSuffix = metricSuffix;
        }

        public double CookedValue { get; private set; }
        public PerformanceCounterType CounterType { get; private set; }
        public uint DefaultScale { get; private set; }
        public string InstanceName { get; private set; }
        public uint MultipleCount { get; private set; }
        public string Path { get; private set; }
        public ulong RawValue { get; private set; }
        public ulong SecondValue { get; private set; }
        public uint Status { get; private set; }
        public ulong TimeBase { get; private set; }
        public DateTime Timestamp { get; private set; }
        public ulong Timestamp100NSec { get; private set; }
        public string MetricSuffix { get; private set; }

        public string MetricPath
        {
            get
            {
                char[] arr = (InstanceName ?? "").Trim().ToCharArray();

                string instance = new string(Array.FindAll<char>(arr, (c => c != ':' && c != '(' && c != ')' && c != '[' && c != ']' && c != '/')));

                instance = instance.Replace('.', '_').Replace(' ', '_').Replace("__", "_");

                return string.Format(MetricSuffix, instance);
            }
        }

        public float MetricValue
        {
            get
            { return Convert.ToSingle(CookedValue); }
        }

        public bool IsFiltered(List<CounterFilter> filters) {
            foreach (CounterFilter filter in filters)
            {
                if (MetricPath.Contains(filter.Expression)) return true;
            }

            return false;
        }
    }
}