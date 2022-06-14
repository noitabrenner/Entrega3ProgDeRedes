using Common.Interfaces;
using System;
using System.Collections.Specialized;
using System.Configuration;

namespace Common
{
    public class SettingsManager : ISettingsManager
    {
        public string ReadSetting(string key)
        {
            try
            {
                NameValueCollection appSettings = ConfigurationManager.AppSettings;
                return appSettings[key] ?? string.Empty;
            }
            catch (ConfigurationErrorsException)
            {
                Console.WriteLine("Error reading the app settings");
                return string.Empty;
            }
        }
    }
}