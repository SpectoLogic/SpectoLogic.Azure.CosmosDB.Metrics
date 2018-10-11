using Newtonsoft.Json;

namespace SpectoLogic.Azure.CosmosDB.Metrics.AzureMonitor.Models
{
    public class LocalizableString
    {
        [JsonProperty(PropertyName = "localizedvalue")]
        public string LocalizedValue { get; set; }
        [JsonProperty(PropertyName = "value")]
        public string Value { get; set; }
    }
}
