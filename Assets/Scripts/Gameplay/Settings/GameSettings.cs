using System.Collections.Generic;
using System.IO;
using DevilMind;
using Newtonsoft.Json;

namespace Fading.Settings
{
    public class GameSettings
    {
        public static Dictionary<int, SkillEffect> SkillOrder
        {
            get { return Instance._skillsOrder; }
        }

        private readonly Dictionary<int, SkillEffect> _skillsOrder;
        private static GameSettings _instance;
        private static GameSettings Instance
        {
            get
            {
                if (_instance == null)
                {
                    LoadSettings();
                }
                return _instance;
            }
        }

        public GameSettings()
        {
            _skillsOrder = new Dictionary<int, SkillEffect>()
            {
                {1, SkillEffect.Catch },
                {2, SkillEffect.ProtectionField },
            };
        }

        public static void LoadSettings()
        {
            var textAsset = ResourceLoader.LoadGameSettings();
            if (textAsset == null)
            {
                _instance = new GameSettings();
                Log.Error(MessageGroup.Common, "Cant load game setting file!");
                return;
            }

            if (string.IsNullOrEmpty(textAsset.text))
            {
                _instance = new GameSettings();
                SaveGameSettings();
                return;
            }

            var loadedSettings = JsonConvert.DeserializeObject<GameSettings>(textAsset.text);
            if (loadedSettings != null)
            {
                _instance = loadedSettings;
            }
        }

        private static void SaveGameSettings()
        {
            var saveInText = JsonConvert.SerializeObject(_instance, Formatting.Indented);
            File.WriteAllText("Assets/Resources/GameSettings.txt", saveInText);
            Log.Info(MessageGroup.Common, saveInText);
        }
    }
}