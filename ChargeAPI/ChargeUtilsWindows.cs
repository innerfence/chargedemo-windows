using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.System;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;

namespace InnerFence.ChargeAPI
{
    public static partial class ChargeUtils
    {
        public static void DeleteLocalData(string key)
        {
            ApplicationData.Current.LocalSettings.Values.Remove(key);
        }

        public static object RetrieveLocalData(string key)
        {
            if( ApplicationData.Current.LocalSettings.Values.ContainsKey(key) )
            {
                return ApplicationData.Current.LocalSettings.Values[key];
            }
               
            return null;
        }

        public static void SaveLocalData(string key, object value)
        {
            ApplicationData.Current.LocalSettings.Values[key] = value;
        }

        public static async void SubmitChargeRequest(ChargeRequest chargeRequest)
        {
            Uri launchURL = chargeRequest.GenerateLaunchURL();

            // Set the launch options in case user doesn't have Credit Card Terminal installed
            var launchOptions = new Windows.System.LauncherOptions();
            launchOptions.PreferredApplicationDisplayName = ChargeRequest.CCTERMINAL_DISPLAY_NAME;
            launchOptions.PreferredApplicationPackageFamilyName = ChargeRequest.CCTERMINAL_PACKAGE_FAMILY_NAME;
            launchOptions.DesiredRemainingView = ViewSizePreference.UseNone;

            var success = await Launcher.LaunchUriAsync(launchURL, launchOptions);

            if (!success)
            {
                throw new ChargeException("Could not launch Credit Card Terminal. Please ensure it has been installed.");
            }
        }
    }
}
