using System;
using UnityEngine;
using System.Collections;
using DevilMind;
using UnityStandardAssets._2D;
using Event = DevilMind.Event;
using EventType = DevilMind.EventType;

namespace Ritualist.Controller 
{
    public class CharacterController : DevilBehaviour
    {
        [SerializeField] private bool _jump, _crouch;

        [SerializeField] private PlatformerCharacter2D _character;
        [SerializeField] private AimBehaviour _aim;
        [Range(0, 2.5f)] [SerializeField] private float _aimRadius;

        private float _xAxisMoveValue;
        private bool _rightTriggerHolded;

        protected override void Awake()
        {
            EventsToListen.Add(EventType.ButtonClicked);
            EventsToListen.Add(EventType.ButtonReleased);
            EventsToListen.Add(EventType.RightTriggerReleased);
            EventsToListen.Add(EventType.RightTriggerClicked);
            base.Awake();
        }

        protected override void OnEvent(Event gameEvent)
        {
            if (GameplayController.Instance.GameEnded)
            {
                return;
            }

            if (gameEvent.Type == EventType.ButtonClicked)
            {
                switch ((InputButton) gameEvent.Parameter)
                {
                    case InputButton.A:
                        Jump();
                        break;
                    case InputButton.B:
                        Debug.Log("CROUCHING");
                        break;
                    case InputButton.X:
                        break;
                }
            }

            if (gameEvent.Type == EventType.RightTriggerClicked)
            {
                _rightTriggerHolded = true;
                //TODO CHANGE INTO GHOST
            }

            if (gameEvent.Type == EventType.RightTriggerReleased)
            {
                _rightTriggerHolded = false;
                //TODO CHANGE INTO RITUALYST
            }
        }

        private void Update()
        {
            if (GameplayController.Instance.GameEnded)
            {
                return;
            }

            SetAimVector();
            Move();
        }

        private void Jump()
        {
            _jump = true;
        }

        private void Move()
        {
            _xAxisMoveValue = MyInputManager.GetAxis(InputAxis.LeftStickX);
            _character.Move(_xAxisMoveValue, false, _jump);
            _jump = false;
        }
        
        private void SetAimVector()
        {
            var position = MyInputManager.GetLeftStickPosition();
            var yPosition = position.y;
            var xPosition = position.x;

            if (Mathf.Abs(yPosition) < 0.2f && Mathf.Abs(xPosition) < 0.2f)
            {
                _aim.Rotate(_character.CharacterFront()*_aimRadius);
                _character.RotateAimOfRuneShooter(_character.CharacterFront() * _aimRadius);
                return;
            }
            _character.RotateAimOfRuneShooter(MyInputManager.GetLeftStickPosition());
            _aim.Rotate(MyInputManager.GetLeftStickPosition());
        }
    }
}

