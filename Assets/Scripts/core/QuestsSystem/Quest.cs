using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace DevilMind.QuestsSystem
{
    [Serializable]
    public class Quest
    {
        public int ID;
        public string Title;
        public string Summary;
        public string LongDescription;
        ///URL can be path to resources as well
        public string Url;
        public List<int> RequairedQuestIds;
        private List<Quest> _requairedQuests;

        public bool IsDone;
        
        public bool IsActive()
        {
            if (IsDone)
            {
                return false;
            }

            var requairedQuests = GetRequairedQuests();
            for (int i = 0, c = requairedQuests.Count; i < c; ++i)
            {
                if (requairedQuests[i].IsDone == false)
                {
                    return false;
                }
            }
            return true;
            
        }
        

        public List<Quest> GetRequairedQuests()
        {
                if (_requairedQuests != null)
                {
                    return _requairedQuests;
                }
                _requairedQuests = new List<Quest>();
                for (int i = 0, c = RequairedQuestIds.Count; i < c; ++i)
                {
                    var id = RequairedQuestIds[i];
                    var quests = GameMaster.QuestManager.Quests;
                    if (quests == null)
                    {
                        return _requairedQuests;
                    }

                    Quest quest;
                    if (quests.TryGetValue(id, out quest))
                    {
                        _requairedQuests.Add(quest);
                    }
                }
                return _requairedQuests;
        }
        
        public Quest(int id)
        {
            ID = id;
            Title = "Uninitialized";
            Summary = "Add Summary";
            LongDescription = "Add Long Description";
            RequairedQuestIds = new List<int>();
        }
    }
}