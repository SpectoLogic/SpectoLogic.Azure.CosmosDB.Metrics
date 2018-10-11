using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace SpectoLogic.Azure.CosmosDB.Metrics.AzureMonitor.Models
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum AggregationType
    {
        [EnumMember]
        None,
        [EnumMember]
        Average,
        [EnumMember]
        Count,
        [EnumMember]
        Minimum,
        [EnumMember]
        Maximum,
        [EnumMember]
        Total
    }
}
