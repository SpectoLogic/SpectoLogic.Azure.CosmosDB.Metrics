using SpectoLogic.Azure.CosmosDB.Metrics.RestProviders.Builder.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace SpectoLogic.Azure.CosmosDB.Metrics.RestProviders.Builder
{
    /// <summary>
    /// 
    /// Example of a typical Azure Monitor Request:
    /// ===========================================
    ///     https://management.azure.com
    ///     /subscriptions/{subscriptionId}/resourceGroups/{resourceGroupName}/providers/Microsoft.DocumentDb/databaseAccounts/{CosmosDBAccountName}/
    ///     providers/microsoft.insights/metrics?
    ///         timespan=2018-10-09T07%3A59%3A00.000Z/2018-10-09T08%3A59%3A00.000Z
    ///        &interval=PT1M
    ///        &metric=TotalRequests
    ///        &aggregation=count
    ///        &$filter=DatabaseName eq 'media-metadata' and CollectionName eq 'media-metadata' and StatusCode eq '*'
    ///        &api-version=2017-05-01-preview
    ///        
    /// </summary>
    public class InsightsQueryBuilder
    {
        const string C_DATE_TIME_FORMAT = "yyy-MM-ddTHH:mm:ss.fffffffZ";        // required by DocumentDB Metric Provider

        private Dictionary<string, string> _QueryParameters = new Dictionary<string, string>();

        /// <summary>
        /// Adds a time filter: startTime eq 2018-10-09T15:09:07.3293392Z and endTime eq 2018-10-09T15:12:07.3293392Z and timeGrain eq duration'PT5M'
        /// </summary>
        /// <param name="period">TimeSpan.FromHours(1)</param>
        /// <param name="interval">"PT5M"</param>
        public InsightsQueryBuilder AddPeriod(TimeSpan period, string interval)
        {
            DateTime start = DateTime.UtcNow.Subtract(period);
            DateTime end = DateTime.UtcNow;
            return AddTime(start,end,interval);
        }

        public InsightsQueryBuilder AddPeriod(TimeSpan period, MetricInterval interval)
        {
            return this.AddPeriod(period, interval.ToString());
        }

        public InsightsQueryBuilder AddTime(DateTime from, DateTime to, string interval)
        {
            string start = from.ToString(C_DATE_TIME_FORMAT);
            string end = to.ToString(C_DATE_TIME_FORMAT);
            _QueryParameters["timespan"] = $"{start}/{end}";
            _QueryParameters["interval"] = interval;
            return this;
        }

        public InsightsQueryBuilder AddTime(DateTime from, DateTime to, MetricInterval interval)
        {
            return this.AddTime(from, to, interval.ToString());
        }

        /// <summary>
        /// Only supported with the new API 2018-01-01 !
        /// For older API-Levels use AddMetric instead.
        /// </summary>
        /// <param name="metric"></param>
        /// <returns></returns>
        public InsightsQueryBuilder AddMetrics(params string[] metric)
        {
            _QueryParameters["metricnames"] = String.Join(",", metric);
            return this;
        }

        public InsightsQueryBuilder AddMetrics(params AzureInsightsMetric[] metric)
        {
            return this.AddMetrics(
                metric.Select(t => { return t.ToMetricString(); }).ToArray<string>()
                );
        }

        /// <summary>
        /// To be used for older API-Levels. For new API 2018-01-01 use AddMetrics.
        /// </summary>
        /// <param name="metric"></param>
        /// <returns></returns>
        public InsightsQueryBuilder AddMetric(string metric)
        {
            if (string.IsNullOrEmpty(metric)) return this;
            _QueryParameters["metric"] = metric;
            return this;
        }

        public InsightsQueryBuilder AddMetric(AzureInsightsMetric metric)
        {
            return this.AddMetric(metric.ToMetricString());
        }

        public InsightsQueryBuilder AggregateBy(string commaSeparatedMetrics)
        {
            if (string.IsNullOrEmpty(commaSeparatedMetrics)) return this;
            _QueryParameters["aggregation"] = commaSeparatedMetrics;
            return this;
        }


        public InsightsQueryBuilder OrderBy(string metric)
        {
            if (string.IsNullOrEmpty(metric)) return this;
            _QueryParameters["orderby"] = metric; // asc desc
            return this;
        }

        public InsightsQueryBuilder Top(int topAmount)
        {
            _QueryParameters["top"] = topAmount.ToString();
            return this;
        }

        /// <summary>
        /// Add an additional OData Query.
        /// </summary>
        /// <param name="oDataQuery"></param>
        /// <returns></returns>
        public InsightsQueryBuilder AddODataQuery(string oDataQuery)
        {
            if (string.IsNullOrEmpty(oDataQuery)) return this;
            _QueryParameters["$filter"] = oDataQuery;
            return this;
        }

        /// <summary>
        /// Add an additional OData Query using the ODataQueryBuilder
        /// </summary>
        /// <param name="oDataQueryBuilder"></param>
        /// <returns></returns>
        public InsightsQueryBuilder AddODataQuery(ODataQueryBuilder oDataQueryBuilder)
        {
            if (oDataQueryBuilder == null) return this;
            if (string.IsNullOrEmpty(oDataQueryBuilder.ToString())) return this;
            _QueryParameters["$filter"] = oDataQueryBuilder.ToString();
            return this;
        }

        /// <summary>
        /// Build the complete Request URL
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder result = new StringBuilder();
            bool isFirst = true;

            foreach (string key in _QueryParameters.Keys)
            {
                if (isFirst)
                    isFirst = false;
                else
                    result.Append("&");
                result.Append($"{key}=");
                result.Append(_QueryParameters[key]);
            }
            return result.ToString();
        }

    }
}
