using UnityEngine;
using System.Collections.Generic;

namespace DevilMind
{
    public delegate void OnObjectStateLoaded(object state);
    public class DevilBehaviour : MonoBehaviour
    {
        [SerializeField][HideInInspector] private string _uniqueID;
        private OnObjectStateLoaded _onObjectStateLoaded;

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
            LoadState();
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

        protected void LoadState(OnObjectStateLoaded onObjectStateLoaded)
        {
            if (onObjectStateLoaded != null)
            {
                _onObjectStateLoaded = onObjectStateLoaded;
            }
        }

        protected void SaveState(object config)
        {
            if (string.IsNullOrEmpty(_uniqueID))
            {
                Log.Warning(MessageGroup.Common,
                    gameObject.name + " is saveable but don't have created unique identifier");
                return;
            }
            var gameSave = GameMaster.GameSave.CurrentSave;
            if (gameSave == null)
            {
                Log.Error(MessageGroup.Common, "Can't get game save");
                return;
            }

            gameSave.InteractiveObjectsStates[_uniqueID] = config;
        }

        private void LoadState()
        {
            if (_onObjectStateLoaded == null)
            {
                return;
            }
            
            if (string.IsNullOrEmpty(_uniqueID))
            {
                Log.Warning(MessageGroup.Common,
                    gameObject.name + " is saveable but don't have created unique identifier");
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
                if (_onObjectStateLoaded != null)
                {
                    _onObjectStateLoaded(null);
                }
                SceneLoader.ObjectsToLoadStateRefCounter--;
                return;
            }

            if (_onObjectStateLoaded != null)
            {
                _onObjectStateLoaded(loadedState);
            }
            SceneLoader.ObjectsToLoadStateRefCounter--;
        }
    }
}