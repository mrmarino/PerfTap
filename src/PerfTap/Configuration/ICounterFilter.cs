using System;
namespace PerfTap.Configuration
{
    public interface ICounterFilter
    {
        string Expression { get; set; }
    }
}
