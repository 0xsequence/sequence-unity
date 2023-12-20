using System.Collections.Generic;

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
        
        public static string RemoveTrailingSlash(this string s)
        {
            if (s.EndsWith("/"))
            {
                s = s.Remove(s.Length - 1);
            }

            return s;
        }

        public static Dictionary<string, string> ExtractQueryParameters(this string url)
        {
            string[] urlSegments = url.Split('?');
            string[] parameters = urlSegments[1].Split('&');
            Dictionary<string, string> queryParameters = new Dictionary<string, string>();
            int totalParameters = parameters.Length;
            for (int i = 0; i < totalParameters; i++)
            {
                string[] keyValue = parameters[i].Split('=');
                queryParameters.Add(keyValue[0], keyValue[1]);
            }

            return queryParameters;
        }
    }
}