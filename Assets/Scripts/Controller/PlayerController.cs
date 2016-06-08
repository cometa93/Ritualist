using System.Runtime.Remoting.Messaging;
using UnityEngine;
using DevilMind;
using Event = DevilMind.Event;
using EventType = DevilMind.EventType;

namespace Fading.Controller 
{
    public class PlayerController : DevilBehaviour
    {
        [SerializeField] private bool _characterMovementEnabled = true;
        [SerializeField] private bool _jump;
        [SerializeField] private MyCharacterController _myCharacterAnimationController;

        private float _xAxisMoveValue;
        private bool _paused;

        protected override void Awake()
        {
            EventsToListen.Add(EventType.ButtonClicked);
            EventsToListen.Add(EventType.ButtonReleased);
            EventsToListen.Add(EventType.RightTriggerReleased);
            EventsToListen.Add(EventType.RightTriggerClicked);
            EventsToListen.Add(EventType.CharacterDied);
            EventsToListen.Add(EventType.CharacterChanged);
            base.Awake();
        }

        protected override void OnEvent(Event gameEvent)
        {
            if (gameEvent.Type == EventType.CharacterDied)
            {
                _characterMovementEnabled = false;
            }
            if (gameEvent.Type == EventType.CharacterChanged)
            {
                _characterMovementEnabled = !_characterMovementEnabled;
            }
       
            if (gameEvent.Type == EventType.ButtonClicked)
            {
                switch ((InputButton) gameEvent.Parameter)
                {
                    case InputButton.Jump:
                        Jump();
                        break;
                    case InputButton.Pause:
                        if (GameplayController.IsGameplayPaused)
                        {
                            return;
                        }
                        PauseGame();
                        break;
                }
            }
        }

        protected override void Update()
        {
            Move();
        }

        private void Jump()
        {
            if (_characterMovementEnabled == false)
            {
                return;
            }
            _jump = true;
        }

        private void Move()
        {
            if (_characterMovementEnabled == false)
            {
                _myCharacterAnimationController.Move(0, false);
                return;
            }
            _xAxisMoveValue = MyInputManager.GetAxis(InputAxis.HorizontalMovement);
            _myCharacterAnimationController.Move(_xAxisMoveValue, _jump);
            _jump = false;
        }

        private void PauseGame()
        {
            GameplayController.IsGameplayPaused = true;
        }
    }
}

