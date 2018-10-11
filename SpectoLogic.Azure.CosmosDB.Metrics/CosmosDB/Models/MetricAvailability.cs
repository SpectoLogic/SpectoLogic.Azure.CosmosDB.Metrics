namespace SpectoLogic.Azure.CosmosDB.Metrics.DocumentDB.Models
{
    public class MetricAvailability
    {
        public BlobLocation BlobLocation { get; set; }
        public MetricLocation Location { get; set; }
        public string Retention { get; set; }
        public string TimeGrain { get; set; }
    }
}
