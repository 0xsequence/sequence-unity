using System.Collections.Generic;

namespace Sequence.Utils
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

        public static Dictionary<string, string> ExtractQueryAndHashParameters(this string url)
        {
            Dictionary<string, string> queryParameters = url.ExtractQueryParameters();
            Dictionary<string, string> hashParameters = url.ExtractHashParameters();
            if (queryParameters == null)
            {
                return hashParameters;
            }
            if (hashParameters == null)
            {
                return queryParameters;
            }
            foreach (KeyValuePair<string, string> hashParameter in hashParameters)
            {
                if (queryParameters.ContainsKey(hashParameter.Key))
                {
                    queryParameters[hashParameter.Key] = hashParameter.Value;
                    continue;
                }
                queryParameters.Add(hashParameter.Key, hashParameter.Value);
            }

            return queryParameters;
        }

        private static Dictionary<string, string> ExtractQueryParameters(this string url)
        {
            return url.ExtractParameters('?');
        }

        private static Dictionary<string, string> ExtractHashParameters(this string url)
        {
            return url.ExtractParameters('#');
        }

        private static Dictionary<string, string> ExtractParameters(this string url, char separator)
        {
            string[] urlSegments = url.Split(separator);
            if (urlSegments.Length != 2)
            {
                return null;
            }
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