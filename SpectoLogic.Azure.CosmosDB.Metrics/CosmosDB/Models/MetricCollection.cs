using Newtonsoft.Json;
using System.Collections.Generic;
using IM = Microsoft.Azure.Insights.Models;

namespace SpectoLogic.Azure.CosmosDB.Metrics.DocumentDB.Models
{
    public class MetricCollection
    {
        public IList<Metric> Value { get; set; }

        public IEnumerable<IM.Metric> ToInsightsMetric()
        {
            List<IM.Metric> result = new List<IM.Metric>();
            foreach (Metric docdbMetric in Value)
            {
                IM.Metric insightsMetric = new IM.Metric();
                insightsMetric.Name = new IM.LocalizableString() { LocalizedValue = docdbMetric.Name.LocalizedValue, Value = docdbMetric.Name.Value };
                insightsMetric.Unit = (IM.Unit)((int)docdbMetric.Unit);
                insightsMetric.Data = new List<IM.MetricValue>();
                if (docdbMetric.MetricValues != null)
                {
                    foreach (MetricValue mV in docdbMetric.MetricValues)
                    {
                        insightsMetric.Data.Add(JsonConvert.DeserializeObject<IM.MetricValue>(JsonConvert.SerializeObject(mV)));
                    }
                }
                result.Add(insightsMetric);
            }
            return result;
        }
    }

}
