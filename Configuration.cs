using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace FmpDataTool
{
    /// <summary>
    /// Configuration
    /// </summary>
    public class Configuration
    {
        private static IConfiguration _configuration;
        private static readonly object lockObject = new object();

        Configuration(){}

        /// <summary>
        /// Instance
        /// </summary>
        public static IConfiguration Instance
        {
            get
            {
                lock (lockObject)
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
}
