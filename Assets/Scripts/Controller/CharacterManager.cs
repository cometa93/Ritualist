using System;
using DevilMind;

namespace Assets.Scripts.Controller
{
    public class CharacterManager : DevilBehaviour
    {
        protected override void Awake()
        {
            EventsToListen.Add(EventType.LeftTriggerReleased);
            base.Awake();
        }

        protected override void OnEvent(Event gameEvent)
        {
            base.OnEvent(gameEvent);
            switch (gameEvent.Type)
            {
                case EventType.LeftTriggerReleased:
                    GameMaster.Events.Rise(EventType.CharacterChanged);
                    break;
            }
        }
    }
}