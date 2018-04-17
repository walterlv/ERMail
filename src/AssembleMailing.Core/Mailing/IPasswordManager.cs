namespace Walterlv.AssembleMailing.Mailing
{
    public interface IPasswordManager
    {
        string Retrieve(string key);

        void Add(string key, string password);
    }
}
