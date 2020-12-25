using System;
using System.Collections.Generic;
using System.Text;

namespace FmpDataTool.Services
{
    public class ConfigService : IConfigService
    {
        public string UrlStockList
        {
            get
            {
                return "https://financialmodelingprep.com/api/v3/stock/list?apikey=14e7a22ed6110f130afa41af05599bb6";
            } 
        }
    }
}
