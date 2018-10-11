using System;
using System.Collections.Generic;
using System.Text;

namespace SpectoLogic.Azure.CosmosDB.Metrics.RestProviders.Builder.Models
{
    /// <summary>
    /// Documentation
    /// https://docs.microsoft.com/en-us/azure/monitoring-and-diagnostics/monitoring-supported-metrics#microsoftdocumentdbdatabaseaccounts
    /// </summary>
    public enum AzureInsightsMetric
    {
        AvailableStorage,           /* not documented */
        CassandraRequestCharges,    /* not documented */
        CassandraRequests,          /* not documented */
        DataUsage,                  /* not documented */
        DocumentCount,              /* not documented */
        DocumentQuota,              /* not documented */
        IndexUsage,                 /* not documented */
        MetadataRequests,
        MongoRequestCharge,
        MongoRequests,
        ReplicationLatency,         /* not documented */
        ServiceAvailability,        /* not documented */
        TotalRequestUnits,
        TotalRequests
    }

    public static class AzureInsightsMetricExtensions
    {
        public static string ToMetricString(this AzureInsightsMetric metricValue)
        {
            return metricValue.ToString();
        }
    }
}
