using System.Collections.Generic;

namespace SpectoLogic.Azure.CosmosDB.Metrics.DocumentDB.Models
{
    public class MetricDefinition
    {
        public IList<Dimension> Dimensions { get; set; }
        public IList<MetricAvailability> MetricAvailabilities { get; set; }
        public LocalizableString Name { get; set; }
        public AggregationType? PrimaryAggregationType { get; set; }
        public IDictionary<string, string> Properties { get; set; }
        string ResourceId { get; set; }
        public Unit Unit { get; set; }
    }
}
