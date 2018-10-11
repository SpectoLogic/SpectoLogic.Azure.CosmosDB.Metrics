using Newtonsoft.Json;
using System;

namespace SpectoLogic.Azure.CosmosDB.Metrics.DocumentDB.Models
{
    public class MetricTableInfo
    {
        public DateTime EndTime { get; set; }
        public string SasToken { get; set; }
        public DateTime SasTokenExpirationTime { get; set; }
        public DateTime StartTime { get; set; }
        public string TableName { get; set; }
    }
}
