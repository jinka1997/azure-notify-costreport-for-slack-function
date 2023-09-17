using Azure.Core;
using Azure.Identity;

namespace CostReportToSlack.Services
{
    public class AccessTokenService
    {
        public static string GetToken(string managedIdentityClientId)
        {
            var credential = new DefaultAzureCredential(new DefaultAzureCredentialOptions()
            {
                //TenantId = tenantId,
                ExcludeVisualStudioCredential = true,
                ManagedIdentityClientId = managedIdentityClientId
            });
            var accessToken = credential.GetToken(
                    new TokenRequestContext(new[] { "https://management.azure.com" })
                );
            var accessTokenString = accessToken.Token.ToString();
            return accessTokenString;
        }

    }
}
