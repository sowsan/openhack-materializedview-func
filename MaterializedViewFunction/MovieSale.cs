using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaterializedViewFunction
{
    public class MovieSale
    {

        [JsonProperty("id")]
        //public int SalesId { get; set; }
        public string MovieId { get; set; }
        public string Name { get; set; }
        public int NumberOfSales { get; set; }
    }
}
