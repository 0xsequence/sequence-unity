namespace SequenceSharp
{
    public class HttpBasicAuthCreds
    {
        public string username;
        public string password;

        public HttpBasicAuthCreds(string username, string password)
        {
            this.username = username;
            this.password = password;
        }
    }
}