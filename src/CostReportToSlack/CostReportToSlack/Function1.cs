using CostReportToSlack.Models;
using CostReportToSlack.Services;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CostReportToSlack
{
    public class Function1
    {

        [FunctionName("Function1")]
        public static async Task Run([TimerTrigger("%ScheduleSetting%")] TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"start:{myTimer.Schedule}");

            var subscriptionId = Environment.GetEnvironmentVariable("SubscriptionId");
            var webhookUrl = Environment.GetEnvironmentVariable("SlackWebhookUrl");
            var dailySummaryByResourceSpan = int.Parse(Environment.GetEnvironmentVariable("DailySummaryByResourceSpan"));
            var managedIdentityClientId = Environment.GetEnvironmentVariable("ManagedIdentityClientId");
            List<CostDetail> costDetails;
            try
            {
                string accessToken = AccessTokenService.GetToken(managedIdentityClientId);
                costDetails = (await CostReportService.GetAsync(subscriptionId, accessToken)).ToList();
            }
            catch (Exception ex)
            {
                log.LogError($"{ex.Message}");
                log.LogError($"{ex.StackTrace}");
                log.LogError($"ó·äOî≠ê∂ èàóùèIóπ");
                return;
            }
            await NotifyService.NotifyDailySummaryAsync(webhookUrl, costDetails);
            await NotifyService.NotifyDailySummaryByResource(webhookUrl, costDetails, dailySummaryByResourceSpan);

            log.LogInformation($"end:{myTimer.Schedule}");
        }
    }
}
