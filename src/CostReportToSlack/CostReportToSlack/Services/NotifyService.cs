using CostReportToSlack.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CostReportToSlack.Services
{
    public class NotifyService
    {
        public static async Task NotifyDailySummaryAsync(string webhookUrl, IEnumerable<CostDetail> costDetails)
        {
            var postMessage = new List<string>
            {
                $"Monthly Total: `{costDetails.Sum(x => x.PreTaxCost):#,##0.000}`",
                "```",
            };

            //日毎のPreTaxCostを合計し、日付昇順に並べる
            var dailySummary = costDetails.GroupBy(x => x.UsageDate)
                                          .Select(x => new
                                          {
                                              UsageDate = x.Key,
                                              Summary = x.Sum(x => x.PreTaxCost)
                                          })
                                          .OrderBy(x => x.UsageDate);
            postMessage.AddRange(dailySummary.Select(x => $"{x.UsageDate:M/dd(ddd)}: {x.Summary:#,##0.000}"));
            postMessage.Add("```");

            await SlackService.PostMessageAsync(webhookUrl, postMessage);
        }

        public static async Task NotifyDailySummaryByResource(string webhookUrl, IEnumerable<CostDetail> message, int dailySummaryByResourceSpan)
        {
            var filterdDetails = message.Where(x => x.UsageDate >= DateTime.Today.AddDays(-1 * dailySummaryByResourceSpan));
            var dateList = filterdDetails.Select(x => x.UsageDate)
                                         .Distinct()
                                         .OrderBy(x => x)
                                         .ToList();

            var postMessage = new List<string>
            {
                $"直近{dailySummaryByResourceSpan}日間の日毎の明細",
                "```",
            };

            foreach (var date in dateList)
            {
                //消費した金額の昇順
                var details = filterdDetails.Where(x => x.UsageDate == date)
                                            .OrderByDescending(x => x.PreTaxCost);

                var text = $"{date:M/dd(ddd)}:{details.Sum(x => x.PreTaxCost):#,##0.000}({string.Join(",", details.Select(x => $"{x.ResourceType.Replace("microsoft.", "")}={x.PreTaxCost:#,##0.000}"))} )";
                postMessage.Add(text);
            }
            postMessage.Add("```");

            await SlackService.PostMessageAsync(webhookUrl, postMessage);
        }
    }
}
