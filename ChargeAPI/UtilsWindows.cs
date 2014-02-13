using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace InnerFence.ChargeAPI
{
    public static partial class Utils
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
    }
}
