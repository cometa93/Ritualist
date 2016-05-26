using System.Collections.Generic;
using DevilMind;
using UnityEngine;
using UnityEngine.UI;
using Event = DevilMind.Event;
using EventType = DevilMind.EventType;

namespace Fading.UI
{
    public class SkillsBarBehaviour : DevilBehaviour
    {

        [SerializeField] private List<Image> _skillIcons;
        [SerializeField] private Color _activeColor;
        [SerializeField] private Color _disabledColor;

        private int _currentSkillIndex;


        protected override void Awake()
        {
            EventsToListen.Add(EventType.ChangeSkill);
            _currentSkillIndex = 1;
            SetupCorrectSkill();
            base.Awake();
        }

        private void SetupCorrectSkill()
        {
            for (int i = 0, c = _skillIcons.Count; i < c; ++i)
            {
                _skillIcons[i].CrossFadeColor(_disabledColor, 0.5f, true, true);
            }

            if (_currentSkillIndex - 1 > _skillIcons.Count || _currentSkillIndex - 1 < 0)
            {
                return;
            }

            _skillIcons[_currentSkillIndex - 1].CrossFadeColor(_activeColor, 0.5f, true, true);
        }

        protected override void OnEvent(Event gameEvent)
        {
            _currentSkillIndex = (int) gameEvent.Parameter;
            SetupCorrectSkill();
            base.OnEvent(gameEvent);
        }
    }
}