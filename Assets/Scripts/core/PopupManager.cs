using System.Collections.Generic;
using UnityEngine;

namespace DevilMind
{
    public static class PopupManager
    {

        public static PopupType CurrentPopup;
        private static readonly LinkedList<PopupType> PopupQueue = new LinkedList<PopupType>();
        private static readonly Dictionary<PopupType, GameObject> InstantionedPopups = new Dictionary<PopupType, GameObject>();

        public static void ShowPopup(PopupType popup)
        {
            if (CurrentPopup != PopupType.Unknown)
            {
                if (InstantionedPopups.ContainsKey(CurrentPopup) == false)
                {
                    Log.Warning(MessageGroup.Common, "Instantioned popups, doesn't contain " + popup);
                    return;
                }
                InstantionedPopups[popup].SetActive(false);
            }

            PopupQueue.AddLast(popup);
            CurrentPopup = popup; 
            if (InstantionedPopups.ContainsKey(CurrentPopup) == false)
            {
                Log.Warning(MessageGroup.Common, "Instantioned popups, doesn't contain " + popup);
                return;
            }

            InstantionedPopups[popup].SetActive(true);
        }

        public static bool IsLoaded(PopupType popup)
        {
            return InstantionedPopups.ContainsKey(popup);
        }

        public static void HidePopup(PopupType type)
        {
            if (PopupQueue.Count > 0)
            {
                PopupQueue.RemoveLast();
                InstantionedPopups[CurrentPopup].SetActive(false);

                if (InstantionedPopups.ContainsKey(CurrentPopup) == false)
                {
                    Log.Warning(MessageGroup.Common, "Instantioned popups, doesn't contain " + type);
                    return;
                }
            }

            CurrentPopup = PopupQueue.Last.Value;
            if (InstantionedPopups.ContainsKey(CurrentPopup) == false)
            {
                Log.Warning(MessageGroup.Common, "Instantioned popups, doesn't contain " + type);
                return;
            }

        }

        public static void HideAllPopups()
        {
            if (InstantionedPopups.ContainsKey(CurrentPopup))
            {
                InstantionedPopups[CurrentPopup].gameObject.SetActive(false);
            }
            PopupQueue.Clear();
        }

        public static void SwitchToLastPopup()
        {
            if (PopupQueue.Count <= 0)
            {
                return;
            }

            InstantionedPopups[CurrentPopup].SetActive(false);
            CurrentPopup = PopupQueue.Last.Value;
            PopupQueue.RemoveLast();
            InstantionedPopups[CurrentPopup].SetActive(true);
        }

        public static void AddNewPopupInstance(KeyValuePair<PopupType, GameObject> popup)
        {
            Log.Info(MessageGroup.Gui, popup.Key + " was loaded !");
            InstantionedPopups.Add(popup.Key, popup.Value);
        }
    }
}