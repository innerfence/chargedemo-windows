using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Navigation;

namespace InnerFence.ChargeAPI
{
    public abstract class ChargeUriMapper : UriMapperBase
    {
        static readonly Regex s_protocolRegex = new Regex(@"^\/Protocol\?encodedLaunchUri=(.+)$");

        static Uri s_lastProtocolUri;
        static DateTime s_lastRedirectedTime = DateTime.MinValue;
        static Uri s_lastRedirectedUri;

        public override Uri MapUri(Uri uri)
        {
            // incoming protocol request uri is of the format:
            // /Protocol?encodedLaunchUri=CHARGE_REQUEST_URL_ENCODED

            // Check to see if uri is a protocol request
            Match protocolMatch = s_protocolRegex.Match(uri.ToString());
            if (protocolMatch.Success)
            {
                if (null != s_lastProtocolUri &&
                    s_lastProtocolUri.Equals(uri) &&
                    DateTime.Now.Subtract(s_lastRedirectedTime) < TimeSpan.FromSeconds(5))
                {
                    // There's a WP8 API bug that calls MapUri twice when activated via protocol
                    // and this check is used to prevent parsing and acting twice
                    return s_lastRedirectedUri;
                }

                s_lastProtocolUri = uri;
                string encodedLaunchUri = HttpUtility.UrlDecode(protocolMatch.Groups[1].Value);
                Uri launchUri = new Uri(encodedLaunchUri);

                // call method to determine where to go / what to do
                s_lastRedirectedUri = GenerateRedirectUri(launchUri);
                s_lastRedirectedTime = DateTime.Now;

                return s_lastRedirectedUri;
            }

            return uri;
        }

        protected abstract Uri GenerateRedirectUri(Uri launchUri);
    }
}
