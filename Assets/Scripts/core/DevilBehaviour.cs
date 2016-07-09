using UnityEngine;
using System.Collections.Generic;
using Fading.InteractiveObjects;

namespace DevilMind
{
    public delegate void OnObjectStateLoaded(object state);
    public class DevilBehaviour : MonoBehaviour
    {
        [SerializeField][HideInInspector] protected string _uniqueID;

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
        }

        protected virtual void Start()
        {    
        }

        protected virtual void OnEnable()
        {
            ListenEvents(EventsToListen);
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

        protected void SaveState<T>(Dictionary<string,T> saveTarget,T config, bool withStats = false)
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

            saveTarget[_uniqueID] = config;
            GameMaster.GameSave.SaveCurrentGameProgress(withStats);
            
        }

        protected void LoadState<T>(Dictionary<string, T> saveTarget, OnObjectStateLoaded onStateLoaded)
        {
            if (onStateLoaded == null)
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

            T loadedState;
            if (saveTarget.TryGetValue(_uniqueID, out loadedState) == false)
            {
                if (onStateLoaded != null)
                {
                    onStateLoaded(null);
                }
                SceneLoader.ObjectsToLoadStateRefCounter--;
                return;
            }

            if (onStateLoaded != null)
            {
                onStateLoaded(loadedState);
            }
            SceneLoader.ObjectsToLoadStateRefCounter--;
        }
    }
}