using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace FmpDataTool.Model
{
    public class Stock
    {
        [JsonPropertyName("symbol")]
        public string Symbol { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("price")]
        public double Price { get; set; }

        [JsonPropertyName("exchange")]
        public string Exchange { get; set; }
    }
}

