using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace SpectoLogic.Azure.CosmosDB.Metrics.DocumentDB.Models
{
    public class MetricValue
    {
        [JsonProperty(PropertyName = "average")]
        public double? Average { get; set; }
        [JsonProperty(PropertyName = "_count")]
        public long? Count { get; set; }
        [JsonProperty(PropertyName = "last")]
        public double? Last { get; set; }
        [JsonProperty(PropertyName = "maximum")]
        public double? Maximum { get; set; }
        [JsonProperty(PropertyName = "minimum")]
        public double? Minimum { get; set; }
        [JsonProperty(PropertyName = "properties")]
        public IDictionary<string, string> Properties { get; set; }
        public DateTime Timestamp { get; set; }
        [JsonProperty(PropertyName = "total")]
        public double? Total { get; set; }
        #region Percentile List Metric
        public double? P10 { get; set; }
        public double? P25 { get; set; }
        public double? P50 { get; set; }
        public double? P75 { get; set; }
        public double? P90 { get; set; }
        public double? P95 { get; set; }
        public double? P99 { get; set; }
        #endregion
    }
}
