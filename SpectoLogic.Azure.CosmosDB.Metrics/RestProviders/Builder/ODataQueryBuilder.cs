using SpectoLogic.Azure.CosmosDB.Metrics.RestProviders.Builder.Models;
using System;
using System.Linq;
using System.Text;

namespace SpectoLogic.Azure.CosmosDB.Metrics.RestProviders.Builder
{
    /// <summary>
    /// Allows to construct a ODataQuery with class methods instead of String concatinations.
    /// </summary>
    public class ODataQueryBuilder
    {
        private StringBuilder _filterBuilder = new StringBuilder();
        private bool _lastOperationOperator = false;

        private bool RequiresOperator()
        {
            if (_filterBuilder.Length == 0) return false;
            return !_lastOperationOperator;
        }

        public ODataQueryBuilder()
        {

        }

        public ODataQueryBuilder And()
        {
            if (!RequiresOperator()) return this; // Ignore
            _filterBuilder.Append(" and ");
            _lastOperationOperator = true;
            return this;
        }
        public ODataQueryBuilder Or()
        {
            if (!RequiresOperator()) return this; // Ignore
            _filterBuilder.Append(" or ");
            _lastOperationOperator = true;
            return this;
        }

        /// <summary>
        /// Example: 
        ///     builder
        ///         .AddMetric("Percentage CPU","Network In");
        ///         
        /// The previous sample will results in following ODATA: (name.value eq 'Percentage CPU' or name.value eq 'Network In')
        /// </summary>
        /// <param name="metricNames"></param>
        /// <returns></returns>
        public ODataQueryBuilder AddMetric(params string[] metricNames)
        {
            if (RequiresOperator()) throw new Exception("Operator 'and' or 'or' expected!");
            var subQueries = metricNames.Select(m => $"name.value eq '{m}'");
            var metricQuery = String.Join(" or ", subQueries);
            _filterBuilder.Append($"({metricQuery})");
            _lastOperationOperator = false;
            return this;
        }
        public ODataQueryBuilder AddMetric(params CosmosDBMetric[] metrics)
        {
            return AddMetric(metrics.Select(m => m.ToMetricString()).ToArray<string>());
        }

        /// <summary>
        /// Adds a time filter: startTime eq 2018-10-09T15:09:07.3293392Z and endTime eq 2018-10-09T15:12:07.3293392Z and timeGrain eq duration'PT5M'
        /// </summary>
        /// <param name="period">TimeSpan.FromHours(1)</param>
        /// <param name="interval">"PT5M"</param>
        public ODataQueryBuilder AddPeriod(TimeSpan period, string interval)
        {
            DateTime start = DateTime.UtcNow.Subtract(period);
            DateTime end = DateTime.UtcNow;
            return this.AddTime(start, end, interval);
        }
        public ODataQueryBuilder AddPeriod(TimeSpan period, MetricInterval interval)
        {
            return this.AddPeriod(period, interval.ToString());
        }

        public ODataQueryBuilder AddTime(DateTime utcFrom, DateTime utcTo, string interval)
        {
            const string dateTimeFormat = "yyy-MM-ddTHH:mm:ss.fffffffZ"; // required by DocumentDB Metric Provider
            if (RequiresOperator()) throw new Exception("Operator 'and' or 'or' expected!");
            string start = utcFrom.ToString(dateTimeFormat);
            string end = utcTo.ToString(dateTimeFormat);
            _filterBuilder.Append($"startTime eq {start} and endTime eq {end} and timeGrain eq duration'{interval}'");
            _lastOperationOperator = false;
            return this;
        }
        public ODataQueryBuilder AddTime(DateTime utcFrom, DateTime utcTo, MetricInterval interval)
        {
            return this.AddTime(utcFrom, utcTo, interval.ToString());
        }

        /// <summary>
        /// DatabaseName eq 'media-metadata' and CollectionName eq 'media-metadata' and StatusCode eq '*'
        /// </summary>
        /// <returns></returns>
        public ODataQueryBuilder AddCosmosDBStatusFilter(string databaseName, string collectionName, string statusCode = "*")
        {
            if (RequiresOperator()) throw new Exception("Operator 'and' or 'or' expected!");
            _filterBuilder.Append($"DatabaseName eq '{databaseName}' and CollectionName eq '{collectionName}' and StatusCode eq '{statusCode}'");
            _lastOperationOperator = false;
            return this;
        }

        public ODataQueryBuilder AddCosmosDBDatabaseFilter(string databaseName)
        {
            if (RequiresOperator()) throw new Exception("Operator 'and' or 'or' expected!");
            _filterBuilder.Append($"DatabaseName eq '{databaseName}'");
            _lastOperationOperator = false;
            return this;
        }

        public ODataQueryBuilder AddCosmosDBCollectionFilter(string collectionName)
        {
            if (RequiresOperator()) throw new Exception("Operator 'and' or 'or' expected!");
            _filterBuilder.Append($"CollectionName eq '{collectionName}'");
            _lastOperationOperator = false;
            return this;
        }

        public ODataQueryBuilder AddCosmosDBStatusFilter(string statuscode)
        {
            if (RequiresOperator()) throw new Exception("Operator 'and' or 'or' expected!");
            _filterBuilder.Append($"StatusCode eq '{statuscode}'");
            _lastOperationOperator = false;
            return this;
        }

        public ODataQueryBuilder AddCustomFilter(string filter)
        {
            if (string.IsNullOrEmpty(filter)) return this;
            if (RequiresOperator()) throw new Exception("Operator 'and' or 'or' expected!");
            _filterBuilder.Append(filter);
            _lastOperationOperator = false;
            return this;
        }

        public override string ToString()
        {
            if (_filterBuilder.Length == 0)
                return null;
            return _filterBuilder.ToString();
        }
    }
}
