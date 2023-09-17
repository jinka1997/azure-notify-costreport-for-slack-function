using CostReportToSlack.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CostReportToSlack.Services
{
    public class CostReportService
    {
        public static async Task<IEnumerable<CostDetail>> GetAsync(string subscriptionId, string accessToken)
        {
            var requestUri = $"https://management.azure.com//subscriptions/{subscriptionId}//providers/Microsoft.CostManagement/query?api-version=2023-03-01";
            var client = new HttpClient();
            var json = JsonSerializer.Serialize(new RequestRoot());

            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(HttpMethod.Post, requestUri);
            request.Headers.Add("Authorization", $"Bearer {accessToken}");
            request.Content = stringContent;

            var response = await client.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                var messageList = new List<string>()
                {
                    "Microsoft.CostManagement/query Failed",
                    $"statusCode={response.StatusCode}",
                    $"ReasonPhrase={response.ReasonPhrase}",
                    $"accessToken={accessToken}"

                };
                throw new Exception(string.Join("\r\n", messageList));
            }
            var costReport = await response.Content.ReadAsAsync<CostReport>();
            var costDetails = GetCostDetails(costReport);
            return costDetails;

        }
        private static IEnumerable<CostDetail> GetCostDetails(CostReport costReport)
        {
            return costReport.Properties.Rows.Select(x => new CostDetail()
            {
                PreTaxCost = decimal.Parse(x[0]),
                UsageDateString = x[1],
                ResourceType = x[2],
                Currency = x[3],
            });
        }
    }
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    //{
    //    "type": "Usage",
    //    "timeframe": "MonthToDate",
    //    "dataset": {
    //        "granularity": "Daily",
    //        "aggregation": {
    //            "totalCost": {
    //                "name": "PreTaxCost",
    //                "function": "Sum"
    //            }
    //        },
    //        "grouping": [
    //            {
    //                "type": "Dimension",
    //                "name": "ResourceType"
    //            }
    //        ]
    //    }
    //}



    #region request

    public class RequestRoot
    {
        [JsonPropertyName("type")]
        public string Type { get; } = "Usage";

        [JsonPropertyName("timeframe")]
        public string Timeframe { get; } = "MonthToDate";

        [JsonPropertyName("dataset")]
        public Dataset Dataset { get; } = new();
    }

    public class Dataset
    {
        [JsonPropertyName("granularity")]
        public string Granularity { get; } = "Daily";

        [JsonPropertyName("aggregation")]
        public Aggregation Aggregation { get; } = new();

        [JsonPropertyName("grouping")]
        public List<Grouping> Grouping { get; } = new() { new Grouping() };
    }

    public class Aggregation
    {
        [JsonPropertyName("totalCost")]
        public TotalCost TotalCost { get; } = new();
    }

    public class TotalCost
    {
        [JsonPropertyName("name")]
        public string Name { get; } = "PreTaxCost";

        [JsonPropertyName("function")]
        public string Function { get; } = "Sum";
    }
    public class Grouping
    {
        [JsonPropertyName("type")]
        public string Type { get; } = "Dimension";

        [JsonPropertyName("name")]
        public string Name { get; } = "ResourceType";
    }
    #endregion

}
