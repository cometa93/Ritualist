using System;
using UnityEngine;
using System.Collections;
using DevilMind;
using UnityStandardAssets._2D;
using Event = DevilMind.Event;
using EventType = DevilMind.EventType;

namespace Ritualist.Controller 
{
    public class PlayerController : DevilBehaviour
    {

        [SerializeField] private bool _jump;
        [SerializeField] private PlatformerCharacter2D _characterAnimationController;
        [SerializeField] private AimBehaviour _aim;
        [Range(0, 2.5f)] [SerializeField] private float _aimRadius;

        private float _xAxisMoveValue;

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
           if (gameEvent.Type == EventType.ButtonClicked)
            {
                switch ((InputButton) gameEvent.Parameter)
                {
                    case InputButton.A:
                        Jump();
                        break;
                    case InputButton.B:
                        break;
                    case InputButton.X:
                        break;
                }
            }
        }

        protected override void Update()
        {
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
            _characterAnimationController.Move(_xAxisMoveValue, false, _jump);
            _jump = false;
        }
        
        private void SetAimVector()
        {
            var position = MyInputManager.GetLeftStickPosition();
            var yPosition = position.y;
            var xPosition = position.x;

            if (Mathf.Abs(yPosition) < 0.2f && Mathf.Abs(xPosition) < 0.2f)
            {
                _aim.Rotate(_characterAnimationController.CharacterFront()*_aimRadius);
                _characterAnimationController.RotateAimOfRuneShooter(_characterAnimationController.CharacterFront() * _aimRadius);
                return;
            }
            _characterAnimationController.RotateAimOfRuneShooter(MyInputManager.GetLeftStickPosition());
            _aim.Rotate(MyInputManager.GetLeftStickPosition());
        }
    }
}

