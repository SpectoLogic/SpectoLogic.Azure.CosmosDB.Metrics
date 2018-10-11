using System.Collections.Generic;

namespace SpectoLogic.Azure.CosmosDB.Metrics.DocumentDB.Models
{
    public class BlobLocation
    {
        public string BlobEndpoint { get; set; }
        public IList<BlobInfo> BlobInfo { get; set; }
    }
}
