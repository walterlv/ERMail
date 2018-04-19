namespace Walterlv.ERMail.OAuth
{
    public class ERMailOutlookOAuth : IOAuthInfo
    {
        public string ClientId { get; } = "00000000482269A3";

        public Tenant Tenant { get; } = "common";

        public Scope Scope { get; } = "openid profile email";
    }
}
