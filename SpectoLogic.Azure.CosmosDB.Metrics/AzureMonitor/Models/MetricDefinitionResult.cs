using Newtonsoft.Json;
using System.Collections.Generic;

namespace SpectoLogic.Azure.CosmosDB.Metrics.AzureMonitor.Models
{
    public class MetricDefinitionResult
    {
        [JsonProperty(PropertyName = "value")]
        public List<MetricDefinition> Value { get; set; }
    }
}
