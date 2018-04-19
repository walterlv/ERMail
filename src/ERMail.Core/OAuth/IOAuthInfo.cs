namespace Walterlv.ERMail.OAuth
{
    public interface IOAuthInfo
    {
        string ClientId { get; }

        Tenant Tenant { get; }

        Scope Scope { get; }
    }
}
