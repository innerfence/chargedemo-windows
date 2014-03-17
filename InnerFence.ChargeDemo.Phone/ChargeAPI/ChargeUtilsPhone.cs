using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Windows.System;

namespace InnerFence.ChargeAPI
{
    public static partial class ChargeUtils
    {
        private static RandomNumberGenerator s_rng;

        public static void DeleteLocalData(string key)
        {
            IsolatedStorageSettings.ApplicationSettings.Remove(key);
        }

        public static object RetrieveLocalData(string key)
        {
            if (IsolatedStorageSettings.ApplicationSettings.Contains(key))
            {
                return IsolatedStorageSettings.ApplicationSettings[key];
            }

            return null;
        }

        public static void SaveLocalData(string key, object value)
        {
            IsolatedStorageSettings.ApplicationSettings[key] = value;
            IsolatedStorageSettings.ApplicationSettings.Save();
        }

        public static async void SubmitChargeRequest(ChargeRequest chargeRequest)
        {
            Uri launchURL = chargeRequest.GenerateLaunchURL();

            var success = await Launcher.LaunchUriAsync(launchURL);

            if (!success)
            {
                throw new ChargeException("Could not launch Credit Card Terminal. Please ensure it has been installed.");
            }
        }

        public static uint GenerateRandomNumber()
        {
            if (s_rng == null)
            {
                s_rng = new RNGCryptoServiceProvider();
            }
            byte[] bytes = new byte[4];
            s_rng.GetBytes(bytes);
            return BitConverter.ToUInt32(bytes, 0);
        }
    }
}
