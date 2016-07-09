using DevilMind;
using UnityEngine;
using Event = DevilMind.Event;
using EventType = DevilMind.EventType;

namespace Fading.InteractiveObjects
{
    public class WorldBehaviour : DevilBehaviour
    {
        private GameObject _myGameObject;

        private bool _isPaused;
        private bool IsPaused
        {
            get { return _isPaused; }
            set
            {
                if (_isPaused == value)
                {
                    return;
                }

                if (value)
                {
                    OnPaused();
                    _isPaused = value;
                    return;
                }

                OnUnpaused();
                _isPaused = value;
            }
        }
        
        protected override void Awake()
        {
            _myGameObject = gameObject;
            EventsToListen.Add(EventType.PauseGame);
            base.Awake();
        }

        private void OnPaused()
        {
            iTween.Pause(_myGameObject, true);
            iTween.Pause(GameplayController.Instance.CharacterTransform, true);
        }

        private void OnUnpaused()
        {
            iTween.Resume(_myGameObject, true);
            iTween.Resume(GameplayController.Instance.CharacterTransform, true);
        }

        protected override void OnEvent(Event gameEvent)
        {
            if (gameEvent.Type == EventType.PauseGame &&
                gameEvent.Parameter is bool)
            {
                IsPaused = (bool) gameEvent.Parameter;
            }
            base.OnEvent(gameEvent);
        }
    }
}