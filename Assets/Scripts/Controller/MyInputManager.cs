using System;
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
        private static bool _controllerRegistered;
        private static bool _rightTriggerClicked;
        private static bool _leftTriggerClicked;

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

        private static readonly Dictionary<int, bool> ClickedButtons = new Dictionary<int, bool>(); 
        private static readonly Dictionary<int, float> ButtonsHoldingTime = new Dictionary<int, float>(); 


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
                if (ClickedButtons.ContainsKey(i) == false)
                {
                    ClickedButtons.Add(i, false);
                    ButtonsHoldingTime.Add(i, 0);
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
                    if (ClickedButtons[i])
                    {
                        GameMaster.Events.Rise(EventType.ButtonReleased, i);
                    }
                    ButtonsHoldingTime[i] = 0;
                    ClickedButtons[i] = false;
                }
                else if (ClickedButtons[i] == false && IsButtonDown((InputButton)i))
                {
                    ClickedButtons[i] = true;
                    GameMaster.Events.Rise(EventType.ButtonClicked, i);
                }
                else if (ClickedButtons[i] && IsButtonDown((InputButton) i))
                {
                    ButtonsHoldingTime[i] += Time.deltaTime;
                }
            }

            //Clicking triggers
            var trigger = GetAxis(InputAxis.LeftTrigger);
            if (Math.Abs(trigger) > 0.01f)
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
            if (Math.Abs(trigger) > 0.01f)
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
