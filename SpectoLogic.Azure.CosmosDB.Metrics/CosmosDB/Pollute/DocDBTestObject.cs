using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace SpectoLogic.Azure.CosmosDB.Metrics.DocumentDB.Pollute
{
    public class DocDBTestObject
    {
        public DocDBTestObject()
        {
            this.Id = Guid.NewGuid().ToString("D");
            this.P = Guid.NewGuid().ToString("D");
            this.FirstName = Guid.NewGuid().ToString("D");
            this.LastName = Guid.NewGuid().ToString("D");
        }
        /// <summary>
        /// Partition Key
        /// </summary>
        [JsonProperty(PropertyName ="p")]
        public string P { get; set; }
        [JsonProperty(PropertyName ="firstname")]
        public string FirstName { get; set; }
        [JsonProperty(PropertyName ="lastname")]
        public string LastName { get; set; }
        [JsonProperty(PropertyName ="id")]
        public string Id { get; set; }
    }
}
