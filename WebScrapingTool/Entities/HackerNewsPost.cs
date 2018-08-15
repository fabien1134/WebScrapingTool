using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebScrapingTool.Entities
{   //Will be used to store each valid post in a collection the serialize to JSON
    public class HackerNewsPost
    {   //The json property name will appear post json serialization regardless of the class property name
        [JsonProperty(PropertyName = "title")]
        public string title { get; set; }
        [JsonProperty(PropertyName = "uri")]
        public string uri { get; set; }
        [JsonProperty(PropertyName = "author")]
        public string author { get; set; }
        [JsonProperty(PropertyName = "points")]
        public int points { get; set; }
        [JsonProperty(PropertyName = "comments")]
        public int comments { get; set; }
        [JsonProperty(PropertyName = "rank")]
        public int rank { get; set; }
    }
}
