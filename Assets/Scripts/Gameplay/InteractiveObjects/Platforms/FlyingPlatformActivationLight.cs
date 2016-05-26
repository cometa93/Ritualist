using DevilMind;
using Fading.Controller;
using UnityEngine;
using Event = DevilMind.Event;
using EventType = DevilMind.EventType;

namespace Fading.InteractiveObjects
{
    public class FlyingPlatformActivationLight : DevilBehaviour
    {
        [SerializeField] FlyingPlatform _platform;

        protected override void Awake()
        {
            EventsToListen.Add(EventType.CharacterChanged);
            base.Awake();    
        }

        protected override void OnEvent(Event gameEvent)
        {
            if (gameEvent.Type == EventType.CharacterChanged)
            {
                _platform.Deactivate();
            }
            base.OnEvent(gameEvent);
        }

        private void OnTriggerEnter2D(Collider2D collider)
        {
            var goodSoul = collider.GetComponent<GoodSoulController>();
            if (goodSoul == null)
            {
                return;
            }
            
            _platform.Activate();
        }

        private void OnTriggerExit2D(Collider2D collider)
        {
            var goodSoul = collider.GetComponent<GoodSoulController>();
            if (goodSoul == null)
            {
                return;
            }
            _platform.Deactivate();
        }
    }
}