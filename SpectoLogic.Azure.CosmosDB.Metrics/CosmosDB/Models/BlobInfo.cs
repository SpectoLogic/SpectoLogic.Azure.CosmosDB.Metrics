using System;

namespace SpectoLogic.Azure.CosmosDB.Metrics.DocumentDB.Models
{
    public class BlobInfo
    {
        public string BlobUri { get; set; }
        public DateTime EndTime { get; set; }
        public string SasToken { get; set; }
        public DateTime SasTokenExpirationTime { get; set; }
        public DateTime StartTime { get; set; }
    }
}
