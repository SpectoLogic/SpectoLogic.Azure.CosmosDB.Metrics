using System;
using System.Text;

namespace SpectoLogic.Azure.CosmosDB.Metrics.Extensions
{
    public static class UriExtensions
    {
        /// <summary>
        /// Sets or replaces a query parameter in an URI.
        /// </summary>
        /// <param name="originalUri"></param>
        /// <param name="parameter"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Uri ReplaceQueryParameter(this Uri originalUri,string parameter, string value)
        {
            string query = originalUri.Query;
            if (!string.IsNullOrEmpty(query))
            {
                StringBuilder newQuery = new StringBuilder();
                newQuery.Append("?");
                bool bIsFirst = true;
                query = query.Substring(1);
                string[] elements = query.Split('&');
                foreach (string element in elements)
                {
                    var elementTokens = element.Split('=');
                    string addTo = "";
                    if (string.Equals(elementTokens[0], parameter,StringComparison.OrdinalIgnoreCase))
                    {
                        addTo = $"parameter={value}";
                    }
                    else
                    {
                        addTo = element;
                    }
                    if (!bIsFirst)
                        newQuery.Append("&");

                    newQuery.Append(addTo);
                    bIsFirst = false;
                }
                string oldAbsoluteUri = originalUri.AbsoluteUri;
                oldAbsoluteUri = oldAbsoluteUri.Substring(0, oldAbsoluteUri.IndexOf('?'));
                return new Uri(oldAbsoluteUri + newQuery.ToString());
            }
            else
                return originalUri;
        }
    }
}
