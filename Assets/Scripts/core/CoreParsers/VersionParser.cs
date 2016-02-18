using System.Collections.Generic;
using Newtonsoft.Json;

namespace DevilMind
{
    public class VersionParser
    {

        public BundleVersion Bundle { private set; get; }
    

        public VersionParser(string json)
        {
            ParseData(json);
        }

        private void ParseData(string json)
        {
            Bundle = JsonConvert.DeserializeObject<BundleVersion>(json) ?? new BundleVersion();
        }

         
    }
}