using System.Collections.Generic;
using System.IO;
using DevilMind;
using DevilMind.QuestsSystem;
using Fading.InteractiveObjects;
using Newtonsoft.Json;

namespace Fading.Settings
{
    public class GameSave
    {
        public GameSave()
        {
            LoadSaveSlots();
            LoadGame(1);
        }

        public Save CurrentSave { private set; get; }

        public readonly List<Save> SaveSlots = new List<Save>(); 

        public class Save
        {
            public readonly int SlotNumber;
            public string CheckpointUniqueId;
            public HeroStats HeroStats;
            public Dictionary<int, Quest> Quests;
            public Dictionary<string, CheckpointConfig> CheckpointsConfigs;
            public Dictionary<string, ActivatedGateConfig> ActivatedDoorsConfig;

            public Save(int slotNumber)
            {
                SlotNumber = slotNumber;
                CheckpointUniqueId = null;
                HeroStats = new HeroStats();
                CheckpointsConfigs = new Dictionary<string, CheckpointConfig>();
                ActivatedDoorsConfig = new Dictionary<string, ActivatedGateConfig>();
                Quests = new Dictionary<int, Quest>();
            }
        }

        public Save LoadGame(int slotNumber)
        {
            if (CurrentSave == null)
            {
                if (SaveSlots.Count == 0)
                {
                    SaveSlots.Add(new Save(slotNumber));
                    SaveCurrentGameProgress();
                }
                CurrentSave = SaveSlots[0];
            }
            return CurrentSave;
//TODO WHEN MENU WILL BE READY CREATE SAVE SLOTS FROM MENU
//            if (SaveSlots.Count < slotNumber && SaveSlots[slotNumber] != null)
//            {
//                CurrentSave = SaveSlots[slotNumber];
//                return SaveSlots[slotNumber];
//            }
//
//            return null;
        }

        public string SaveCurrentGameProgress(bool withStats = true)
        {
            if (CurrentSave != null && withStats)
            {
                CurrentSave.HeroStats = GameMaster.Hero.Stats.CurrentStats();
            }
            var saveInText = JsonConvert.SerializeObject(SaveSlots, Formatting.Indented);
            File.WriteAllText("Assets/Resources/GameStateSave.txt", saveInText);
            return saveInText;
        }

        public void LoadCurrentGame()
        {
            if (CurrentSave == null)
            {
                Log.Error(MessageGroup.Gameplay, "Can't load game Current save is null");
                return;
            }

            if (CurrentSave.CheckpointUniqueId == null)
            {
                SceneLoader.Instance.LoadClearStage(1);
                return;
            }

            CheckpointConfig config;
            if(CurrentSave.CheckpointsConfigs.TryGetValue(CurrentSave.CheckpointUniqueId, out config) == false)
            {
                SceneLoader.Instance.LoadClearStage(1);
                return;
            }

            CheckpointConfig checkpointData = config as CheckpointConfig;
            if (checkpointData == null)
            {
                Log.Warning(MessageGroup.Gameplay, "Can't cast config to checkpoint config something is bad");
                SceneLoader.Instance.LoadClearStage(1);
                return;
            }


            SceneLoader.Instance.LoadClearStage(checkpointData.StageNumber);
        }

        private void LoadSaveSlots()
        {
            var textAsset = ResourceLoader.LoadGameSave();
            if (textAsset == null)
            {
                Log.Error(MessageGroup.Common, "Cant load game saves file!");
                return;
            }

            if (string.IsNullOrEmpty(textAsset.text))
            {
                return;
            }

            var slotList = JsonConvert.DeserializeObject<List<Save>>(textAsset.text);
            if (slotList != null)
            {
                slotList.Sort((slot, save1) => slot.SlotNumber < save1.SlotNumber ? 1 : -1);
                SaveSlots.Clear();
                SaveSlots.AddRange(slotList);
            }
        }


#region Quest Manager Delegates

        public void SaveQuests(Dictionary<int, Quest> questsToSave)
        {
            if (CurrentSave == null)
            {
                Log.Error(MessageGroup.Gameplay, "Can't save quests in game current save is null");
                return;
            }

            CurrentSave.Quests = questsToSave;
            var saveInText = JsonConvert.SerializeObject(SaveSlots, Formatting.Indented);
            File.WriteAllText("Assets/Resources/GameStateSave.txt", saveInText);
        }

        public bool LoadQuests(out Dictionary<int, Quest> quests)
        {
            if (CurrentSave == null)
            {
                Log.Error(MessageGroup.Gameplay, "Can't save quests in game current save is null");
                quests = new Dictionary<int, Quest>();
                return false;
            }

            quests = CurrentSave.Quests;
            return true;
        }

#endregion
    }
}