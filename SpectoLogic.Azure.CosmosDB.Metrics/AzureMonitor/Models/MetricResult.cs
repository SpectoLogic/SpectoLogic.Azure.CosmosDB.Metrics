using Newtonsoft.Json;
using System.Collections.Generic;

namespace SpectoLogic.Azure.CosmosDB.Metrics.AzureMonitor.Models
{
    public class MetricResult
    {
        [JsonProperty(PropertyName = "cost")]
        public int Cost { get; set; }
        [JsonProperty(PropertyName = "timespan")]
        public string Timespan { get; set; }
        [JsonProperty(PropertyName = "interval")]
        public string Interval { get; set; }
        [JsonProperty(PropertyName = "value")]
        public List<AzureMonitor.Models.Metric> Value { get; set; }
        [JsonProperty(PropertyName ="namespace")]
        public string NameSpace { get; set; }
        [JsonProperty(PropertyName = "resourceregion")]
        public string ResourceRegion { get; set; }
    }
}
