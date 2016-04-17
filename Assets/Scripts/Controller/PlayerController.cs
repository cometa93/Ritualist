﻿using System.Runtime.Remoting.Messaging;
using UnityEngine;
using DevilMind;
using Event = DevilMind.Event;
using EventType = DevilMind.EventType;

namespace Ritualist.Controller 
{
    public class PlayerController : DevilBehaviour
    {
        [SerializeField] private bool _characterMovementEnabled = true;
        [SerializeField] private bool _jump;
        [SerializeField] private CharacterController _characterAnimationController;

        private float _xAxisMoveValue;

        protected override void Awake()
        {
            EventsToListen.Add(EventType.ButtonClicked);
            EventsToListen.Add(EventType.ButtonReleased);
            EventsToListen.Add(EventType.RightTriggerReleased);
            EventsToListen.Add(EventType.RightTriggerClicked);
            EventsToListen.Add(EventType.CharacterDied);
            base.Awake();
        }

        protected override void OnEvent(Event gameEvent)
        {
            if (gameEvent.Type == EventType.CharacterDied)
            {
                _characterMovementEnabled = false;
                
            }

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
                _characterAnimationController.Move(0, false);
                return;
            }
            _xAxisMoveValue = MyInputManager.GetAxis(InputAxis.LeftStickX);
            _characterAnimationController.Move(_xAxisMoveValue, _jump);
            _jump = false;
        }
    }
}

