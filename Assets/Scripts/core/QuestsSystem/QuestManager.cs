using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace DevilMind.QuestsSystem
{
    public class QuestManager
    {
        private const string _resourcesURL = "QuestManager/PreparedQuests";

        public QuestManager(
            LoadQuestsDelegate loadQuests,
            SaveQuestsDelegate saveQuests)
        {
            if (loadQuests == null)
            {
                Log.Error(MessageGroup.Common, "load quests delegate cannot be null. Initialization cannot be complete");
                return;
            }

            if (saveQuests == null)
            {
                Log.Error(MessageGroup.Common, "save quests delegate cannot be null. Initialization cannot be complete");
                return;
            }

            _loadQuests = loadQuests;
            _saveQuests = saveQuests;
        }

        public class QuestsSave
        {
            public int CurrentQuestId;
            public int LastId;
            public Dictionary<int, Quest> Quests;

            public QuestsSave()
            {
                Quests = new Dictionary<int, Quest>();
                LastId = 0;
            }
        }

        public delegate void SaveQuestsDelegate(Dictionary<int,Quest> _questsToSave );
        public delegate bool LoadQuestsDelegate(out Dictionary<int, Quest> loadedQuests);

        private Dictionary<int, Quest> _quests;
        public Dictionary<int, Quest> Quests
        {
            get
            {
                if (_quests == null)
                {
                    GetQuests();
                    return null;
                }

                return _quests;
            }
            set
            {
                if (_quests == null)
                {
                    _quests = value;
                }
            }
        }

        private readonly LoadQuestsDelegate _loadQuests;
        private readonly SaveQuestsDelegate _saveQuests;

        private bool CheckForInitialization()
        {
            if (_loadQuests == null)
            {
                Log.Error(MessageGroup.Common, "load quests delegate cannot be null. Initialization cannot be complete");
                return false;
            }

            if (_saveQuests == null)
            {
                Log.Error(MessageGroup.Common, "save quests delegate cannot be null. Initialization cannot be complete");
                return false;
            }

            return true;
        }

        private void GetQuests()
        {
            if (CheckForInitialization() == false)
            {
                return;
            }

            var allQuestsPrepared = GetQuestsFromResources();
            if (allQuestsPrepared == null)
            {
                return;
            }
            
            if (_loadQuests(out _quests) == false)
            {
                _quests = new Dictionary<int, Quest>();
            }

            foreach (var quest in allQuestsPrepared)
            {
                if (_quests.ContainsKey(quest.Key))
                {
                    continue;
                }
                _quests.Add(quest.Key, quest.Value);
            }

        }

        private Dictionary<int,Quest> GetQuestsFromResources()
        {
            var textAsset = ResourceLoader.Load<TextAsset>(_resourcesURL);
            if (textAsset == null || string.IsNullOrEmpty(textAsset.text))
            {

                Log.Error(MessageGroup.Common, "Quest Manager have to be initialized first before use.");
                return null;
            }

            var questSave = JsonConvert.DeserializeObject<QuestsSave>(textAsset.text, Settings.JsonSerializerSettings());
            if (questSave == null)
            {
                Log.Error(MessageGroup.Common, "There is no serialized quests prepared for game");
                return null;
            }

            return questSave.Quests;
        }

        public void Save()
        {
            if (_saveQuests == null)
            {
                Log.Error(MessageGroup.Gameplay, "Save quests delegate is null");
                return;
            }

            _saveQuests(_quests);
        }
    }
}