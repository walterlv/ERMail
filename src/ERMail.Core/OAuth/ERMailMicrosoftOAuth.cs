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

        public Scope Scope { get; } = "openid profile email";

        public string MakeUrl()
        {
            return $@"https://login.microsoftonline.com/{Tenant}/oauth2/v2.0/authorize
?client_id={ClientId}
&scope={Scope}";
        }
    }
}
