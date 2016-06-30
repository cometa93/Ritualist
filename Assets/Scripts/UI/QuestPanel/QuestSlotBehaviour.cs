using System;
using DevilMind;
using DevilMind.QuestsSystem;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Fading.UI
{
    public class QuestSlotBehaviour
    {
        [SerializeField] private Text _questTitle;

        private Action _onSlotSelected;

        public void Setup(Quest quest, Action onSelected)
        {
            _questTitle.text = quest.Title;
            _onSlotSelected = onSelected;
        }

        public void OnSelect(BaseEventData eventData)
        {
            if (_onSlotSelected == null)
            {
                Log.Error(MessageGroup.Gui, "no on selected event is set in quest slot");
                return;
            }

            _onSlotSelected();
        }
    }
}