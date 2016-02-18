using DevilMind;
using DevilMind.Utils;
using UnityEngine;
using Event = DevilMind.Event;
using EventType = DevilMind.EventType;

namespace Ritualist.UI
{
    public class HealthBar : DevilBehaviour
    {
        [SerializeField] private UIProgressBar _healthProgressBar;

        protected override void Awake()
        {
            _healthProgressBar.Value = (float)GameMaster.Hero.Stats.Health / GameMaster.Hero.Stats.MaxHealth;
            EventsToListen.Add(EventType.HeroHurted);
            base.Awake();
        }

        protected override void OnEvent(Event gameEvent)
        {
            if (gameEvent.Type == EventType.HeroHurted)
            {
                _healthProgressBar.Value = (float)GameMaster.Hero.Stats.Health / GameMaster.Hero.Stats.MaxHealth;
            }
            base.OnEvent(gameEvent);
        }
    }
}