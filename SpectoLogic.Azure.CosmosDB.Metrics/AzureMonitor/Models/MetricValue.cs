namespace SpectoLogic.Azure.CosmosDB.Metrics.AzureMonitor.Models
{
    public class MetricValue
    {
        public string timestamp { get; set; }
        public double? total { get; set; }
        public double? average { get; set; }
        public long? count { get; set; }
        public double? maximum { get; set; }
        public double? minimum { get; set; }
    }
}
