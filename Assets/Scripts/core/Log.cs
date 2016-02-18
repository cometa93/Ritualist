using System;
namespace DevilMind
{
    #region MessageTypes

    public enum MessageType
    {
        Info,
        Debug,
        Error,
        Warning,
        Exception
    }


    public enum MessageGroup
    {
        Network,
        Gameplay,
        Gui,
        Common
    }

    #endregion

    public class Log
    {
        public static void Info(MessageGroup messageGroup, string message)
        {
#if UNITY_EDITOR
            UnityEngine.Debug.Log("MESSAGE ADDRESER : "+ messageGroup + ", MESSAGE: " +message);
#endif
        }

        public static void Error(MessageGroup messageGroup, string message, bool forceSend = false)
        {
           UnityEngine.Debug.LogError("MESSAGE ADDRESER : " + messageGroup + ", MESSAGE: " + message);
        }

        public static void Exception(MessageGroup messageGroup, string message, bool forceSend = true)
        {
           UnityEngine.Debug.LogException(new Exception("EXCEPTION ADDRESER : " + messageGroup + ", MESSAGE: " + message));
           UnityEngine.Debug.Break();
        }

        public static void Warning(MessageGroup messageGroup, string message)
        {
#if UNITY_EDITOR
            UnityEngine.Debug.LogWarning("MESSAGE ADDRESER : " + messageGroup + ", MESSAGE: " + message);
#endif
        }

        public static void Debug(MessageGroup messageGroup, string message)
        {

#if UNITY_EDITOR
            UnityEngine.Debug.Log("MESSAGE ADDRESER : " + messageGroup + ", MESSAGE: " + message);
#endif
        }
    }
}