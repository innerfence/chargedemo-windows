using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnerFence.ChargeAPI
{
    public static partial class ChargeUtils
    {
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

        public static void SubmitChargeRequest(ChargeRequest chargeRequest)
        {
            throw new NotImplementedException();
        }

        public static uint GenerateRandomNumber()
        {
            throw new NotImplementedException();
        }
    }
}
