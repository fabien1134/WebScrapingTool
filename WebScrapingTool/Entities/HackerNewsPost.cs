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
        private bool m_isValid = true;

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

        //Lazy load property
        [JsonIgnore]
        public bool IsValid
        {
            get
            {   //Ensure the title and author meet the requirements
                m_isValid = (ValidateString(title, author)) ? m_isValid : false;
                m_isValid = (ValidateInt(points, comments, rank)) ? m_isValid : false;
                m_isValid = Uri.IsWellFormedUriString(uri, UriKind.RelativeOrAbsolute) ? m_isValid : false;
                return m_isValid;
            }
        }


        public bool ValidateInt(int points, int comments, int rank)
        {
            return IsIntValid(points) && IsIntValid(comments) && IsIntValid(rank);

            //Local Method
            bool IsIntValid(int input)
            {
                return input >= 0;
            }
        }

        private bool ValidateString(string title, string author)
        {
            return IsStringValid(title) && IsStringValid(author);

            //Local Method
            bool IsStringValid(string input)
            {
                bool inputValid = false;

                if (!string.IsNullOrEmpty(input))
                    if (input.Length <= 256)
                        inputValid = true;

                return inputValid;
            }
        }
    }
}
