using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace SpectoLogic.Azure.CosmosDB.Metrics.DocumentDB.Models
{
    public class Metric
    {
        public LocalizableString DimensionName { get; set; }
        public LocalizableString DimensionValue { get; set; }
        [JsonProperty(PropertyName = "endTime")]
        public DateTime EndTime { get; set; }
        [JsonProperty(PropertyName = "metricValues")]
        public IList<MetricValue> MetricValues { get; set; }
        [JsonProperty(PropertyName = "name")]
        public LocalizableString Name { get; set; }
        [JsonProperty(PropertyName = "properties")]
        public IDictionary<string, string> Properties { get; set; }
        [JsonProperty(PropertyName = "resourceid")]
        public string ResourceId { get; set; }
        [JsonProperty(PropertyName = "partitionId")]
        public string PartitionId { get; set; }
        [JsonProperty(PropertyName = "partitionKeyRangeId")]
        public string PartitionKeyRangeId { get; set; }
        [JsonProperty(PropertyName = "startTime")]
        public DateTime StartTime { get; set; }
        [JsonProperty(PropertyName = "timeGrain")]
        public string TimeGrain { get; set; }
        [JsonProperty(PropertyName = "unit")]
        public Unit Unit { get; set; }
    }
}
