using System.Collections.Generic;

namespace SpectoLogic.Azure.CosmosDB.Metrics.DocumentDB.Models
{
    public class MetricLocation
    {
        public string PartitionKey { get; set; }
        public string TableEndpoint { get; set; }
        public IList<MetricTableInfo> TableInfo { get; set; }
    }
}
