using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System;

namespace InnerFence.ChargeAPI
{
    public class ChargeRequest
    {
        const string CCTERMINAL_BASE_URL = @"com-innerfence-ccterminal://charge/1.0.0/";

        public ChargeRequest()
        {

        }

        public async void Submit()
        {
            string uriToLaunch = @"com-innerfence-ccterminal://charge/1.0.0/";

            // TODO: generate params
            
            // Launch the URI
            var uri = new Uri(uriToLaunch);
            var success = await Launcher.LaunchUriAsync(uri);

            if( !success )
            {
                throw new InvalidOperationException("Failed to launch Credit Card Terminal. Please make sure app is installed.");
            }
        }
    }
}
