using Newtonsoft.Json;
using System.Runtime.Serialization;
using Newtonsoft.Json.Converters;

namespace SpectoLogic.Azure.CosmosDB.Metrics.AzureMonitor.Models
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum Unit
    {
        [EnumMember]
        Count,
        [EnumMember]
        Bytes,
        [EnumMember]
        Seconds,
        [EnumMember]
        CountPerSecond,
        [EnumMember]
        BytesPerSecond,
        [EnumMember]
        Percent,
        [EnumMember]
        MilliSeconds
    }
}
