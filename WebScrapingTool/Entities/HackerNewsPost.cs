using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebScrapingTool.Entities
{   //Will be used to store each valid post in a collection the serialize to JSON
    public class HackerNewsPost
    {
        public string title { get; set; }
        public string uri { get; set; }
        public string author { get; set; }
        public int points { get; set; }
        public int comments { get; set; }
        public int rank { get; set; }
    }
}
