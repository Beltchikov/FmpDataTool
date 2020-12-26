using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace FmpDataTool
{
    public class Configuration
    {
        private static IConfiguration _configuration;

        public static IConfiguration Instance
        {
            get
            {
                if (_configuration == null)
                {
                    _configuration = new ConfigurationBuilder().AddJsonFile("appSettings.json").Build();
                }
                return _configuration;
            }
        }
    }
}
