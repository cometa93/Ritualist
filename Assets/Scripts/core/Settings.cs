using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace DevilMind
{
    public class Settings
    {

        public static JsonSerializerSettings JsonSerializerSettings()
        {
            var jsonSerializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
            return jsonSerializerSettings;
        }

    }
}