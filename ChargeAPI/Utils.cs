using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnerFence.ChargeAPI
{
    public static partial class Utils
    {
        private const string NONCE_ALPHABET = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-_";
        private const int NONCE_LENGTH = 27; // same size as base64-encoded SHA1 seems good

        public static string GenerateNonce()
        {
            Random random = new Random();
            var nonceString = new StringBuilder();
            for (int i = 0; i < NONCE_LENGTH; i++)
            {
                nonceString.Append(NONCE_ALPHABET[random.Next(0, NONCE_ALPHABET.Length - 1)]);
            }

            return nonceString.ToString();
        }

        public static Dictionary<string, string> ParseQueryString(string query)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();

            query = query.TrimStart('?');
            if (!String.IsNullOrEmpty(query))
            {
                string[] paramArray = query.Split('&');
                foreach (string param in paramArray)
                {
                    string[] paramPair = param.Split('=');
                    string key = null;
                    string value = null;

                    if (paramPair.Length == 1)
                    {
                        key = paramPair[0];
                    }
                    else if (paramPair.Length == 2)
                    {
                        key = paramPair[0];
                        value = paramPair[1];
                    }
                    else
                    {
                        throw new Exception(String.Format("Invalid param: {0}", param));
                    }

                    key = Uri.UnescapeDataString(key);
                    value = Uri.UnescapeDataString(value);
                    parameters[key] = value;
                }
            }
            return parameters;
        }

        public static Uri UriWithAdditionalParams(Uri uri, Dictionary<string, string> newParameters)
        {
            Dictionary<string, string> parameters;
            if (String.IsNullOrEmpty(uri.Query))
            {
                parameters = newParameters;
            }
            else
            {
                // extract existing query string parameters
                parameters = Utils.ParseQueryString(uri.Query);

                // add new parameters -- new parameters will overwrite old ones if key already exists
                foreach (var param in newParameters)
                {
                    parameters[param.Key] = param.Value;
                }
            }

            if (parameters.Count == 0)
            {
                // no parameters = no query string.
                return uri;
            }

            // loop through parameters to generate param list
            List<string> paramList = new List<string>();
            foreach (var param in parameters)
            {
                if (param.Value != null)
                {
                    paramList.Add(
                        String.Format("{0}={1}",
                        Uri.EscapeDataString(param.Key),
                        Uri.EscapeDataString(param.Value))
                    );
                }
            }

            // join param list with & and prefix it with ?
            string queryString = String.Join("&", paramList);
            queryString = String.Format("?{0}", queryString);

            string uriString = String.Format(
                "{0}://{1}{2}{3}",
                uri.Scheme,
                uri.Host,
                uri.AbsolutePath,
                queryString);

            return new Uri(uriString);
        }
    }
}
