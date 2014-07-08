using BuildingApi;
using Microsoft.WindowsAzure;
using NLog;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using Flurl;
using System.Threading;

namespace WattApp.WebJobs
{
    class Program
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();
        private static ITokenProvider _tokenProvider = null;
        private static string _baseAPIRoot = string.Empty;

        static void Main(string[] args)
        {
            try
            {
                _initClientAPI();
                Token token = _tokenProvider.Get();
                IEnumerable<Company> companies = HttpHelper.Get<Company[]>(_baseAPIRoot.AppendPathSegment("companies").ToString(), token);
                _logger.Info("Number of Companies: " + companies.Count());
            }
            catch (Exception e)
            {
                _logger.Error("Unhandle Exception", e);
            }
            Thread.Sleep(1000);
        }

        private static void _initClientAPI()
        {
            var clientId = Settings.Get("JciClientId", null, s => !string.IsNullOrEmpty(s), "You must specify a JCI-issued client id in the .config file.");
            var clientSecret = Settings.Get("JciClientSecret", null, s => !string.IsNullOrEmpty(s), "You must specify a JCI-issued client secret in the .config file.");
            if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret))
                throw new ArgumentException("JciClientId and/or JciClientSecret is not valid. Add them to the .config file settings section.");
            var tokenEndpoint = Settings.Get("JciTokenEndpoint", "https://identity.johnsoncontrols.com/issue/oauth2/token",
                s => new Uri(s).IsAbsoluteUri,
                "JciTokenEndpoint in configuration settings should be the URL where JCI's tokens are issued from.");
            _baseAPIRoot = Settings.Get("JciBuildingApiEndpoint", "https://api.panoptix.com/building",
                s => new Uri(s).IsAbsoluteUri,
                "JciBuildingApiEndpoint in configuration settings should be the base URL of the Building API.");

            _tokenProvider = new TokenClient(clientId, clientSecret, tokenEndpoint, WebProxy.GetDefaultProxy());
        }
    }

    public static class Settings
    {
        public static int Get(string settingKey, int defaultValueIfNotFound)
        {
            int i = -1;
            Get(settingKey,
                defaultValueIfNotFound.ToString(CultureInfo.InvariantCulture),
                str => Int32.TryParse(str, out i),
                settingKey + " in config app setting must be an integer.");
            return i;
        }

        public static string Get(string settingKey, string defaultValueIfNotFound)
        {
            return Get(settingKey, defaultValueIfNotFound, s => true, null);
        }

        public static string Get(string settingKey, string defaultValueIfNotFound, Predicate<string> validityCheck, string warningMessageIfCheckFails)
        {
            var settingValue = GetSettingFromFile(settingKey) ?? defaultValueIfNotFound;
            if (!validityCheck(settingValue) && !string.IsNullOrEmpty(warningMessageIfCheckFails))
            {
                Log.Warn(warningMessageIfCheckFails);
            }
            return settingValue;
        }

        private delegate string GetSettingDelegate(string key);
        private static readonly GetSettingDelegate GetSettingFromFile = CloudConfigurationManager.GetSetting;
        private static readonly Logger Log = LogManager.GetLogger("Jci.Panoptix.HistoricalCollector.ConfigSettings");
    }

}
