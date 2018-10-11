using System;
using System.Collections.Generic;
using System.Text;

namespace SpectoLogic.Azure.CosmosDB.Metrics.RestProviders.Builder.Models
{
    /// <summary>
    /// Be aware that not all metrics allow every interval!
    /// </summary>
    public enum MetricInterval
    {
        PT1M,
        PT5M,
        PT1H,
        P1D
    }
}
