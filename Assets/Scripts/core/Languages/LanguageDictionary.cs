using System.Collections.Generic;

namespace DevilMind.Languages
{
    public class LanguageDictionary
    {


        private static readonly Dictionary<LanguageKey,string> _languageDictionary = new Dictionary<LanguageKey, string>
        {
            {LanguageKey.B_REGISTER, "Register"},
            {LanguageKey.B_EXPLORE, "Explore" }

        }; 

        public static string Get(LanguageKey key)
        {
            if (_languageDictionary.ContainsKey(key))
            {
                return _languageDictionary[key];
            }

            Log.Warning(MessageGroup.Common, "Language dictionary doesn't contains key: " + key);
            return "";
        }
 

    }
}