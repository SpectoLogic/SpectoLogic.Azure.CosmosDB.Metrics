using System;
using System.Collections.Generic;
using System.Text;

namespace SpectoLogic.Azure.CosmosDB.Metrics.AzureMonitor.Models
{
    public class Metric
    {
        public string id { get; set; }
        public string type { get; set; }
        public LocalizableString name { get; set; }
        public string unit { get; set; }
        public List<TimeSeries> timeseries { get; set; }
    }
}
