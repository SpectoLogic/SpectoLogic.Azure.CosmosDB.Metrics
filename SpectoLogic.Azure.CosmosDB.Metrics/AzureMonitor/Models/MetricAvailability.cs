using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SpectoLogic.Azure.CosmosDB.Metrics.AzureMonitor.Models
{
    public class MetricAvailability
    {
        [JsonProperty(PropertyName = "retention")]
        public string Retention { get; set; }
        [JsonProperty(PropertyName = "timegrain")]
        public string TimeGrain { get; set; }
    }
}
