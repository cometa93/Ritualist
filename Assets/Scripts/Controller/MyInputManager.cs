    using System;
using UnityEngine;
using System.Collections.Generic;
using DevilMind;
using EventType = DevilMind.EventType;

// ReSharper disable once CheckNamespace
namespace Fading.Controller
{
    public static class MyInputManager
    {
        public const float Radius = 6f;
        private const float SpeedModyfierValue = 0.3f;
        private static bool _controllerRegistered;
        private static bool _rightTriggerClicked;
        private static bool _leftTriggerClicked;

        private static bool IsInputDeviceConnected
        {
            get { return Input.GetJoystickNames().Length > 0; }
        }

        private static readonly Dictionary<InputAxis, string> AxisNames = new Dictionary<InputAxis, string>
        {
            {InputAxis.LeftStickX, "LeftX"},
            {InputAxis.RightStickX, "RightX" },
            {InputAxis.LeftStickY, "LeftY"},
            {InputAxis.RightStickY, "RightY" },
            {InputAxis.LeftTrigger, "LeftTrigger" },
            {InputAxis.RightTrigger, "RightTrigger" },
            {InputAxis.SkillXAxis, "SkillXAxis" },
            {InputAxis.SkillYAxis, "SkillYAxis" }
        };

        private static readonly Dictionary<InputButton, string> ButtonNames = new Dictionary<InputButton, string>
        {
            {InputButton.A, "A" },
            {InputButton.B, "B" },
            {InputButton.X, "X" },
        };

        private static readonly Dictionary<InputButton, string> ButtonKeyboardNames = new Dictionary
            <InputButton, string>
        {
            {InputButton.A, "Aaction" },
            {InputButton.B, "Baction" },
            {InputButton.X, "Xaction" }
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
            var input = Input.GetAxis(AxisNames[axis]);
            return Math.Abs(input) > 0.05f ? input : GetKeyboardAxis(axis);
        }

        public static int GetRawAxis(InputAxis axis)
        {
            if (IsInputDeviceConnected)
            {
                var result = (int) Input.GetAxisRaw(AxisNames[axis]);
                if (result != 0)
                {
                    return result;
                }
            }

            if (axis == InputAxis.SkillXAxis ||
                axis == InputAxis.SkillYAxis)
            {
                return (int) GetKeyboardSkillValue(axis);
            }

            return (int) Mathf.Round(GetKeyboardAxis(axis));
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
            ClickedButtons.Add((int) InputButton.SkillButton1, false);
            ClickedButtons.Add((int) InputButton.SkillButton2, false);
            ClickedButtons.Add((int) InputButton.SkillButton3, false);
            ClickedButtons.Add((int) InputButton.SkillButton4, false);
            _controllerRegistered = true;
        }

        private static float GetKeyboardAxis(InputAxis axis)
        {
            switch (axis)
            {
               
                case InputAxis.LeftStickX:
                    return GetKeyboardMoveValue();

                case InputAxis.LeftStickY:
                    return GetKeyboardUpDownValue();

                case InputAxis.LeftTrigger:
                    return Input.GetButton("CharacterChange") ? 1f : 0f;

                case InputAxis.RightTrigger:
                    return Input.GetButton("Shoot") ? 1f : 0f;

                default:
                    Log.Error(MessageGroup.Common, axis + "dont have represenatation on keyboard");
                    break;
            }

            return 0f;
        }

        private static float GetKeyboardSkillValue(InputAxis axis)
        {
            switch (axis)
            {
                case InputAxis.SkillXAxis:
                    if (Input.GetButton("Skill2Button"))
                    {
                        return 1f;
                    }
                    if (Input.GetButton("Skill4Button"))
                    {
                        return -1f;
                    }
                    return 0;

                case InputAxis.SkillYAxis:
                    if (Input.GetButton("Skill1Button"))
                    {
                        return 1f;
                    }
                    if (Input.GetButton("Skill3Button"))
                    {
                        return -1f;
                    }
                    return 0;
            }

            return 0;
        }

        private static float GetKeyboardMoveValue()
        {
            var initValue = 0f;
            if (Input.GetButton("Right"))
            {
                initValue += 1f;
            }
            if (Input.GetButton("Left"))
            {
                initValue -= 1f;
            }

            if (Input.GetButton("Speed Modyficator"))
            {
                initValue *= SpeedModyfierValue;
            }

            return initValue;
        }

        private static float GetKeyboardUpDownValue()
        {
            var initValue = 0f;
            if (Input.GetButton("Up"))
            {
                initValue += 1f;
            }
            if (Input.GetButton("Down"))
            {
                initValue -= 1f;
            }

            if (Input.GetButton("Speed Modyficator"))
            {
                initValue *= SpeedModyfierValue;
            }

            return -initValue;
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

            if (ButtonKeyboardNames.ContainsKey(buttonType) == false)
            {
                Debug.LogWarning("There is no button key name in dict. " + buttonType);
                return false;
            }

            return Input.GetButtonDown(ButtonNames[buttonType]) || Input.GetButtonDown(ButtonKeyboardNames[buttonType]);
        }

        public static void Update()
        {
            if (_controllerRegistered == false)
            {
                RegisterController();
                return;
            }

            IsSkillButtonDown(InputButton.SkillButton1);
            IsSkillButtonDown(InputButton.SkillButton2);
            IsSkillButtonDown(InputButton.SkillButton3);
            IsSkillButtonDown(InputButton.SkillButton4);

            //Actions for each button.
            for (int i = 1, c = (int) InputButton.Count; i < c; ++i)
            {
                if (IsButtonDown((InputButton) i) == false)
                {
                    if (ClickedButtons[i])
                    {
                        GameMaster.Events.Rise(EventType.ButtonReleased, i);
                    }
                    ButtonsHoldingTime[i] = 0;
                    ClickedButtons[i] = false;
                }
                else if (ClickedButtons[i] == false && IsButtonDown((InputButton) i))
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

                float xPosition = leftStickX*Radius;

                float leftStickY = GetAxis(InputAxis.LeftStickY);
                float yPosition = -leftStickY*Radius;

                return new Vector3(xPosition, yPosition);
            }
        }

        public static Vector3 GetRightStickPosition()
        {
            float rightStickX = GetAxis(InputAxis.RightStickX);

            float xPosition = rightStickX*Radius;

            float rightStickY = GetAxis(InputAxis.RightStickY);
            float yPosition = -rightStickY*Radius;

            return new Vector3(xPosition, yPosition);
        }

        private static void IsSkillButtonDown(InputButton buttonType)
        {
            if (buttonType != InputButton.SkillButton1 && buttonType != InputButton.SkillButton2 && buttonType != InputButton.SkillButton3 && buttonType != InputButton.SkillButton4)
            {
                return;
            }

            var axis = buttonType == InputButton.SkillButton1 || buttonType == InputButton.SkillButton3 ? InputAxis.SkillYAxis : InputAxis.SkillXAxis;

            int positiveValue = buttonType == InputButton.SkillButton1 || buttonType == InputButton.SkillButton2 ? 1 : -1;
            var buttonKeyIndex = (int) buttonType;

            if (GetRawAxis(axis) == 0)
            {
                if (ClickedButtons[buttonKeyIndex])
                {
                    GameMaster.Events.Rise(EventType.ButtonReleased, buttonType);
                }
                ButtonsHoldingTime[buttonKeyIndex] = 0;
                ClickedButtons[buttonKeyIndex] = false;
            }
            else if (ClickedButtons[buttonKeyIndex] == false && GetRawAxis(axis) == positiveValue)
            {
                ClickedButtons[buttonKeyIndex] = true;
                GameMaster.Events.Rise(EventType.ButtonClicked, buttonType);
            }
            else if (ClickedButtons[buttonKeyIndex] && GetRawAxis(axis) == positiveValue)
            {
                ButtonsHoldingTime[buttonKeyIndex] += Time.deltaTime;
            }
        }
    }
}
