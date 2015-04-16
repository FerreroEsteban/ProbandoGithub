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
    }
}
