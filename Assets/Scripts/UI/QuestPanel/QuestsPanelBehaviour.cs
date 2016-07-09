using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DevilMind;
using DevilMind.QuestsSystem;
using DevilMind.Utils;
using UnityEngine.UI;

namespace Fading.UI
{

    public class QuestsPanelBehaviour : DevilBehaviour
    {
        [SerializeField] private GameObject _questSlotPrefab;
        [SerializeField] private GameObject _questSlotsParent;
        [SerializeField] private Text _questDescription;
        [SerializeField] private Text _questTitle;

        private readonly Dictionary<int, QuestSlotBehaviour> _spawnedSlots = new Dictionary<int, QuestSlotBehaviour>();

        protected override void OnEnable()
        {
            Setup();
            base.OnEnable();
        }

        public void Setup()
        {
            SetupSlots();
        }

        private void SetupSlots()
        {
            var quests = GameMaster.QuestManager.Quests;
            int number = 0;
            if (quests == null)
            {
                return;
            }
            
            foreach (var quest in quests)
            {
                var slot = GetSlot(number++);
                if (slot == null)
                {
                    continue;
                }

                Quest questToSelect = quest.Value;
                slot.Setup(quest.Value, () =>
                {
                    SlotSelected(questToSelect);
                });
                if (number == 1)
                {
                    SlotSelected(quest.Value);
                }
            }

        }

        private QuestSlotBehaviour GetSlot(int nr)
        {
            QuestSlotBehaviour slot;
            if (_spawnedSlots.TryGetValue(nr, out slot))
            {
                return slot;
            }

            var spawned = Instantiate(_questSlotPrefab);
            spawned.transform.SetLayer(_questSlotsParent);
            spawned.transform.SetParent(_questSlotsParent.transform);
            spawned.transform.localScale = Vector3.one;
            spawned.transform.localEulerAngles = Vector3.zero;

            slot = spawned.GetComponent<QuestSlotBehaviour>();
            if (slot == null)
            {
                Log.Error(MessageGroup.Gui, "Quest Slot dont have correct behaviour");
                Destroy(spawned);
                return null;
            }

            _spawnedSlots[nr] = slot;
            return slot;
        }

        private void SlotSelected(Quest quest)
        {
            _questTitle.text = quest.Title;
            _questDescription.text = quest.Summary;
        }
    }
}
