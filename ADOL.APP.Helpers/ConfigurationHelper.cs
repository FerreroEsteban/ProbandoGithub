using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace ADOL.APP.CurrentAccountService.Helpers
{
    public static class ConfigurationHelper
    {
        public static string GetConfigurationItem(string itemKey)
        {
            return ConfigurationManager.AppSettings.Get(itemKey) ?? string.Empty;
        }

        private static string eventsServiceURL;
        public static string EventsServiceURL
        {
            get 
            {
                if (string.IsNullOrEmpty(eventsServiceURL))
                    eventsServiceURL = ConfigurationHelper.GetConfigurationItem("eventServiceURL");
                return eventsServiceURL;
            }
            private set 
            { 
                //do nothing
            }
        }

        private static string resultServiceURL;
        public static string ResultServiceURL
        {
            get
            {
                if (string.IsNullOrEmpty(resultServiceURL))
                    resultServiceURL = ConfigurationHelper.GetConfigurationItem("resultServiceURL");
                return resultServiceURL;
            }
            private set
            {
                //do nothing
            }
        }

        private static string serviceUSR;
        public static string ServiceUSR
        {
            get
            {
                if (string.IsNullOrEmpty(serviceUSR))
                    serviceUSR = ConfigurationHelper.GetConfigurationItem("serviceUSR");
                return serviceUSR;
            }
            private set
            {
                //do nothing
            }
        }

        private static string servicePWD;
        public static string ServicePWD
        {
            get
            {
                if (string.IsNullOrEmpty(servicePWD))
                    servicePWD = ConfigurationHelper.GetConfigurationItem("servicePWD");
                return servicePWD;
            }
            private set
            {
                //do nothing
            }
        }
    }
}
