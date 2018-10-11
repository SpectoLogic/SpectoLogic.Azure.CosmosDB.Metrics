using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SpectoLogic.Azure.CosmosDB.Metrics.AzureMonitor.Models
{
    public class Metadata
    {
        [JsonProperty(PropertyName = "name")]
        public LocalizableString Name { get; set; }
        [JsonProperty(PropertyName = "value")]
        public string Value { get; set; }
    }
}
