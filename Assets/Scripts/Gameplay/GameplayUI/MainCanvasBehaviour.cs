using System.Collections.Generic;
using DevilMind;
using DevilMind.Utils;
using SRF;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Fading.UI
{
    public class MainCanvasBehaviour : MonoBehaviour
    {
        [SerializeField] Transform _canvasParent;
        [SerializeField] EventSystem _uiEventSystem;

        private static MainCanvasBehaviour _instance;
        public static MainCanvasBehaviour Instance { get { return _instance;} }

        public static EventSystem EventSystem
        {
            get { return _instance != null ? _instance._uiEventSystem : null; }
        }


        private readonly Dictionary<UIType,GameObject> _registeredPanels = new Dictionary<UIType, GameObject>();
        
        public void Setup()
        {
            if (_instance != null)
            {
                Log.Warning(MessageGroup.Common, "Already registered main canvas");
                return;
            }
            _instance = this;
            DontDestroyOnLoad(this);
        }

        public static GameObject RegisterPanel(UIType panelType, GameObject prefab)
        {
            if (_instance._registeredPanels.ContainsKey(panelType))
            {
                Log.Warning(MessageGroup.Gui, "UI panel already registered type : " + panelType);
                return null;
            }
            var gameObject = Instantiate(prefab);
            if (gameObject == null)
            {
                Log.Error(MessageGroup.Common, "Cant instantiate gameobject panel");
                return null;
            }

            DontDestroyOnLoad(gameObject);
            _instance._registeredPanels[panelType] = gameObject;
            gameObject.transform.SetLayer(_instance.gameObject);
            gameObject.transform.SetParent(_instance._canvasParent,false);
          
            return gameObject;
        }

        public static bool TryGetRegisteredPanel(UIType panelType, out GameObject panel)
        {
            return _instance._registeredPanels.TryGetValue(panelType, out panel);
        }

        public static void DisablePanel(UIType panelType)
        {
            GameObject panel;
            if (_instance._registeredPanels.TryGetValue(panelType, out panel))
            {
                if (panel != null)
                {
                    panel.gameObject.SetActive(false);
                }
                return;
            }
            Log.Warning(MessageGroup.Gui, "Can't disable panel by type: " + panelType);
        }

        public static void EnablePanel(UIType panelType)
        {
            GameObject panel;
            if (_instance._registeredPanels.TryGetValue(panelType, out panel))
            {
                if (panel != null)
                {
                    panel.gameObject.SetActive(true);
                }
                return;
            }
            Log.Warning(MessageGroup.Gui, "Can't enable panel by type: " + panelType);
        }
    }
}