using System.Collections.Generic;

namespace SpectoLogic.Azure.CosmosDB.Metrics.DocumentDB.Models
{
    public class Dimension
    {
        public LocalizableString Name { get; set; }
        public IList<LocalizableString> Values { get; set; }
    }
}
