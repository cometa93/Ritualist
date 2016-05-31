using UnityEngine;
using System.Collections.Generic;

namespace DevilMind
{
    public class DevilBehaviour : MonoBehaviour
    {
        private string _uniqueID;
        private bool _saveable;

        protected readonly List<EventType> EventsToListen = new List<EventType>(); 

        protected virtual void OnEvent(Event gameEvent)
        {
        }

        protected virtual void UpdateLanguage()
        {
        }

        protected void ListenEvents(List<EventType> eventsToListen)
        {
            for (int i = 0, c = eventsToListen.Count; i < c; ++i)
            {
                GameMaster.Events.AddListener(eventsToListen[i],OnEvent);
            }
        }

        protected virtual void Awake()
        {
            ListenEvents(EventsToListen);
        }

        protected virtual void Start()
        {      
        }

        protected virtual void OnEnable()
        {
        }

        protected virtual void Update()
        {
        }

        protected virtual void OnDisable()
        {
            
            foreach (var eventType in EventsToListen)
            {
                GameMaster.Events.RemoveListener(eventType, OnEvent);
            }
            Reset();
        }

        protected virtual void Reset()
        {
        }

        private void SaveState()
        {
            _saveable = true;
        }

        private void LoadState(System.Action<Object> onStateLoaded)
        {
            if (_saveable == false)
            {
                return;
            }

            SceneLoader.ObjectsToLoadStateRefCounter++;
            var gameSave = GameMaster.GameSave.CurrentSave;
            if (gameSave == null)
            {
                Log.Error(MessageGroup.Common, "Can't get game save");
                SceneLoader.ObjectsToLoadStateRefCounter--;
                return;
            }
            object loadedState;
            if (gameSave.InteractiveObjectsStates.TryGetValue(_uniqueID, out loadedState) == false)
            {
                onStateLoaded = null;
            }
        }
    }
}