using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Identity.Client;

namespace Walterlv.ERMail.OAuth
{
    /// <summary>
    /// Stores OAuth info of outlook.com.
    /// See https://docs.microsoft.com/en-us/azure/active-directory/develop/active-directory-v2-scopes fore more details.
    /// </summary>
    public class ERMailMicrosoftOAuth : IOAuthInfo
    {
        public string ClientId { get; } = "00000000482269A3";

        public Tenant Tenant { get; } = "common";

        public Scope Scope { get; } = "openid profile offline_access email";

        public string ResponseType { get; } = "code";

        public Uri MakeUrl()
        {
            var uri = new Uri($@"https://login.microsoftonline.com/{Tenant}/oauth2/v2.0/authorize
?client_id={ClientId}
&&response_type={ResponseType}
&scope={Scope}");
            return uri;
        }

        public async Task<string> AcquireTokenAsync()
        {
            var publicClientApp = new PublicClientApplication(ClientId);
            AuthenticationResult authResult;

            try
            {
                authResult =
                    await publicClientApp.AcquireTokenSilentAsync(Scope, publicClientApp.Users.FirstOrDefault());
            }
            catch (MsalUiRequiredException ex)
            {
                // A MsalUiRequiredException happened on AcquireTokenSilentAsync. This indicates you need to call AcquireTokenAsync to acquire a token
                System.Diagnostics.Debug.WriteLine($"MsalUiRequiredException: {ex.Message}");

                try
                {
                    authResult = await publicClientApp.AcquireTokenAsync(Scope);
                }
                catch (MsalException msalex)
                {
                    // ResultText.Text = $"Error Acquiring Token:{System.Environment.NewLine}{msalex}";
                    throw;
                }
            }
            catch (Exception ex)
            {
                // ResultText.Text = $"Error Acquiring Token Silently:{System.Environment.NewLine}{ex}";
                throw;
            }

            return authResult.AccessToken;
            //ResultText.Text = await GetHttpContentWithToken(_graphAPIEndpoint, authResult.AccessToken);
            //DisplayBasicTokenInfo(authResult);
            //this.SignOutButton.Visibility = Visibility.Visible;
        }
    }
}
