using InnerFence.ChargeAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnerFence.ChargeDemo.Phone
{
    public class AssociationUriMapper : ChargeUriMapper
    {
        protected override Uri GenerateRedirectUri(Uri launchUri)
        {
            // This sample always uses com-innerfence-chargedemo://chargeresponse
            // as the base return URL.
            if (launchUri.Host.Equals("chargeresponse"))
            {
                App.Current.HandleResponse(launchUri);                
            }

            // In your app, this is where you define which view you want the
            // returning charge request call to land on.
            return new Uri("/MainPage.xaml", UriKind.Relative);
        }
    }
}
