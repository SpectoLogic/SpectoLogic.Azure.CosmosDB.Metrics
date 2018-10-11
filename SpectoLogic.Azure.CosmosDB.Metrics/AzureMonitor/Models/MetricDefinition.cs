using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SpectoLogic.Azure.CosmosDB.Metrics.AzureMonitor.Models
{
    public class MetricDefinition
    {
        [JsonProperty(PropertyName ="id")]
        public string Id { get; set; }
        [JsonProperty(PropertyName = "resourceid")]
        public string ResourceId { get; set; }
        [JsonProperty(PropertyName = "metricavailabilities")]
        public List<MetricAvailability> MetricAvailabilities { get; set; }
        [JsonProperty(PropertyName = "name")]
        public LocalizableString Name { get; set; }
        [JsonProperty(PropertyName = "primaryaggregationtype")]
        public AggregationType? PrimaryAggregationType { get; set; }
        [JsonProperty(PropertyName = "unit")]
        public Unit? Unit { get; set; }
    }
}
