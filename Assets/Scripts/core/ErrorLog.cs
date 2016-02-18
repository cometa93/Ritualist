using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DevilMind
{
    public class ErrorLog
    {
        private const string KEY_LAST_SAVED_INDEX = "last_saved_index";
        private const string KEY_LAST_SEND_INDEX = "last_sended_index";
        private const string KEY_LOG_SAVE = "LOG_NR_";

        private const int SECONDS_TO_RESEND = 10;


        private static float _timeSinceLastTry = 0;

        static ErrorLog()
        {
            #region ENABLING LOG

#if !UNITY_EDITOR
            Enabled = true;
                #else
            Enabled = false;
#endif

            #endregion

            Application.logMessageReceived += HandleDebugLog;

            LastSavedIndex = PlayerPrefs.HasKey(KEY_LAST_SAVED_INDEX)
                ? PlayerPrefs.GetInt(KEY_LAST_SAVED_INDEX)
                : 0;
            LastSendedIndex = PlayerPrefs.HasKey(KEY_LAST_SEND_INDEX)
                ? PlayerPrefs.GetInt(KEY_LAST_SEND_INDEX)
                : 0;
        }

        public static bool Enabled { set; get; }
        private static int LastSavedIndex { set; get; }
        private static int LastSendedIndex { set; get; }

        private static void HandleDebugLog(string message, string stackTrace, LogType messageLogType)
        {
           //if (Enabled)
           if(messageLogType == LogType.Error)
           {
                var error = new ErrorInfo
                {
                    Date = DateTime.Today.ToString(),
                    Device = Device.ID,
                    Message = message + "\n\n STACK TRACE : \n" + stackTrace,
                    Version = GameMaster.Instance.Bundle + "\n Build Number: " + GameMaster.Instance.BuildNumber
                };
                StoreLog(error);
           }
        }

        private static void StoreLog(ErrorInfo errorInfo)
        {
            LastSavedIndex++;
            if (LastSavedIndex == 200)
            {
                LastSavedIndex = 0;
            }
            PlayerPrefs.SetString(KEY_LOG_SAVE + LastSavedIndex, errorInfo.Serialize());
            PlayerPrefs.SetInt(KEY_LAST_SAVED_INDEX, LastSavedIndex);
        }

        private static ErrorInfo GetLog()
        {

            if (PlayerPrefs.HasKey(KEY_LOG_SAVE + (LastSendedIndex+1)))
            {
                string notParsedErrorInfo = PlayerPrefs.GetString(KEY_LOG_SAVE +(LastSendedIndex+1));
                ErrorInfo error = ErrorInfo.Deserialize(notParsedErrorInfo);
                return error;
            }
            return null;
        }

        private class ErrorInfo
        {
            public int Id { set; get; }
            public string Device { set; get; }
            public string Date { set; get; }
            public string Version { set; get; }
            public string Message { set; get; }

            public string Serialize()
            {
                return "Serialized Error";
            }

            public static ErrorInfo Deserialize(string json)
            {
                //TODO: DESERIALIZE;
               return  new ErrorInfo();
            }
        }

        public static void Update()
        {
           // if (!Enabled)
          //  {
          //      return;
          //  }
              
            _timeSinceLastTry += Time.deltaTime;
            if (_timeSinceLastTry >= SECONDS_TO_RESEND)
            {
                _timeSinceLastTry = 0;
                ErrorInfo info = GetLog();
                if (info == null)
                {
                    LastSendedIndex++;
                    return;
                }
                RequestManager.SendError(info.Message + "\n", request =>
                {
                    if (request.Success())
                    {
                        LastSendedIndex++;
                        PlayerPrefs.DeleteKey(KEY_LOG_SAVE+LastSendedIndex);
                        if (LastSavedIndex == LastSendedIndex)
                        {
                            LastSavedIndex = 0;
                            LastSendedIndex = 0;
                        }
                    }
                });
            }
            
        }

    }
}