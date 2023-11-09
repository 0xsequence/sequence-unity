namespace Sequence.Authentication
{
    public static class UrlExtensions
    {
        public static string AppendTrailingSlashIfNeeded(this string s)
        {
            if (s != null && s.Length > 0 && !s.EndsWith('/'))
            {
                return s + '/';
            }

            return s;
        }
    }
}