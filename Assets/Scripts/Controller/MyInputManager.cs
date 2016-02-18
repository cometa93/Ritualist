using UnityEngine;
using System.Collections.Generic;
using DevilMind;
using EventType = DevilMind.EventType;

// ReSharper disable once CheckNamespace
namespace Ritualist.Controller
{
    public static class MyInputManager
    {
        public const float Radius = 6f;
        private static bool _controllerRegistered = false;
        private static bool _rightTriggerClicked = false;
        private static bool _leftTriggerClicked = false;
        private static float _triggerHoldTime = 0;

        private static readonly Dictionary<InputAxis, string> AxisNames = new Dictionary<InputAxis, string>
        {
            {InputAxis.LeftStickX, "LeftX"},
            {InputAxis.RightStickX, "RightX" },
            {InputAxis.LeftStickY, "LeftY"},
            {InputAxis.RightStickY, "RightY" },
            {InputAxis.LeftTrigger, "LeftTrigger" },
            {InputAxis.RightTrigger, "RightTrigger" }
        };

        private static readonly Dictionary<InputButton, string> ButtonNames = new Dictionary<InputButton, string>
        {
            {InputButton.A, "A" },
            {InputButton.B, "B" },
            {InputButton.X, "X" }
        };

        private static Dictionary<int, bool> _clickedButtons = new Dictionary<int, bool>(); 
        private static Dictionary<int, float> _buttonsHoldingTime = new Dictionary<int, float>(); 


        public static float GetAxis(InputAxis axis)
        {
            if (axis == InputAxis.Unknown)
            {
                Debug.LogWarning("Given axis is unknown");
                return 0;
            }
            if (AxisNames.ContainsKey(axis) == false)
            {
                Debug.LogWarning("There is no axis key name in dict.");
                return 0;
            }

            return Input.GetAxis(AxisNames[axis]);
        }

        private static void RegisterController()
        {
            for (int i = 1, c = (int)InputButton.Count; i < c; ++i)
            {
                if (_clickedButtons.ContainsKey(i) == false)
                {
                    _clickedButtons.Add(i, false);
                    _buttonsHoldingTime.Add(i, 0);
                }
            }
            _controllerRegistered = true;
        }

        public static bool IsButtonDown(InputButton buttonType)
        {
            if (buttonType == InputButton.Unknown)
            {
                Debug.LogWarning("Given axis is unknown");
                return false;
            }
            if (ButtonNames.ContainsKey(buttonType) == false)
            {
                Debug.LogWarning("There is no button key name in dict. " + buttonType);
                return false;
            }

            return Input.GetButtonDown(ButtonNames[buttonType]);
        }

        public static void Update()
        {
            if (_controllerRegistered == false)
            {
                RegisterController();
                return;
            }
            
            //Actions for each button.
            for (int i = 1, c = (int)InputButton.Count; i < c; ++i)
            {
                if (IsButtonDown((InputButton)i) == false)
                {
                    if (_clickedButtons[i])
                    {
                        GameMaster.Events.Rise(EventType.ButtonReleased, i);
                    }
                    _buttonsHoldingTime[i] = 0;
                    _clickedButtons[i] = false;
                }
                else if (_clickedButtons[i] == false && IsButtonDown((InputButton)i))
                {
                    _clickedButtons[i] = true;
                    GameMaster.Events.Rise(EventType.ButtonClicked, i);
                }
                else if (_clickedButtons[i] && IsButtonDown((InputButton) i))
                {
                    _buttonsHoldingTime[i] += Time.deltaTime;
                }
            }

            //Clicking triggers
            var trigger = GetAxis(InputAxis.LeftTrigger);
            if (trigger != 0)
            {
                if (trigger > 0 && _leftTriggerClicked == false)
                {
                    _leftTriggerClicked = true;
                    GameMaster.Events.Rise(EventType.LeftTriggerClicked);
                }
            }
            else
            {
                if (_leftTriggerClicked)
                {
                    _leftTriggerClicked = false;
                    GameMaster.Events.Rise(EventType.LeftTriggerReleased);
                }
            }

            trigger = GetAxis(InputAxis.RightTrigger);
            if (trigger != 0)
            {
                if (trigger > 0 && _rightTriggerClicked == false)
                {
                    _rightTriggerClicked = true;
                    GameMaster.Events.Rise(EventType.RightTriggerClicked);
                }
            }
            else
            {
                if (_rightTriggerClicked)
                {
                    _rightTriggerClicked = false;
                    GameMaster.Events.Rise(EventType.RightTriggerReleased);
                }
            }



        }

        public static Vector3 GetLeftStickPosition()
        {
            {
                float leftStickX = GetAxis(InputAxis.LeftStickX);

                float xPosition = leftStickX * Radius;

                float leftStickY = GetAxis(InputAxis.LeftStickY);
                float yPosition = -leftStickY * Radius;

                return new Vector3(xPosition, yPosition);
            }
        }

        public static Vector3 GetRightStickPosition()
        {
            float rightStickX = GetAxis(InputAxis.RightStickX);

            float xPosition = rightStickX * Radius;

            float rightStickY = GetAxis(InputAxis.RightStickY);
            float yPosition = -rightStickY * Radius;

            return new Vector3(xPosition, yPosition);
        }
    }
}
