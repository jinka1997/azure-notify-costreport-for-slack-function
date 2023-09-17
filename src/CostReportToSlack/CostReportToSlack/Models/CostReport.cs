using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CostReportToSlack.Models
{
    public class CostReport
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("location")]
        public object Location { get; set; }

        [JsonPropertyName("sku")]
        public object Sku { get; set; }

        [JsonPropertyName("eTag")]
        public object ETag { get; set; }

        [JsonPropertyName("properties")]
        public Properties Properties { get; set; }
    }


    public class Column
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }
    }

    public class Properties
    {
        [JsonPropertyName("nextLink")]
        public object NextLink { get; set; }

        [JsonPropertyName("columns")]
        public List<Column> Columns { get; set; }

        [JsonPropertyName("rows")]
        public List<List<string>> Rows { get; set; }
    }

    public class CostDetail
    {
        public decimal PreTaxCost { get; set; }
        public string UsageDateString { get; set; }

        public DateTime UsageDate
        {
            get
            {
                var year = int.Parse(UsageDateString[..4]);
                var month = int.Parse(UsageDateString.Substring(4, 2));
                var day = int.Parse(UsageDateString.Substring(6, 2));
                return new DateTime(year, month, day);
            }
        }

        public string ResourceType { get; set; }
        public string Currency { get; set; }
    }

}
