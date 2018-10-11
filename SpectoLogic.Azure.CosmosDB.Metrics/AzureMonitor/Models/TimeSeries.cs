using Newtonsoft.Json;
using System.Collections.Generic;

namespace SpectoLogic.Azure.CosmosDB.Metrics.AzureMonitor.Models
{
    public class TimeSeries
    {
        [JsonProperty(PropertyName = "metadatavalues")]
        public List<Metadata> MetadataValues { get; set; }
        [JsonProperty(PropertyName = "data")]
        public List<MetricValue> Data { get; set; }
    }
}
