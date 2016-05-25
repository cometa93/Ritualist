using System.Collections.Generic;
using DevilMind;
using DevilMind.Utils;
using UnityEngine;

namespace Fading.UI
{
    public class MainCanvasBehaviour : MonoBehaviour
    {
        private static MainCanvasBehaviour _instance;
        public static MainCanvasBehaviour Instance { get { return _instance;} }

        public void Register()
        {
            if (_instance != null)
            {
                Log.Warning(MessageGroup.Common, "Already registered main canvas");
                return;
            }
            _instance = this;
            DontDestroyOnLoad(this);
        }

        private readonly Dictionary<UIType,GameObject> _registeredPanels = new Dictionary<UIType, GameObject>();

        [SerializeField] Transform _canvasParent;

        public static void RegisterPanel(UIType panelType, GameObject prefab)
        {
            if (_instance._registeredPanels.ContainsKey(panelType))
            {
                Log.Warning(MessageGroup.Gui, "UI panel already registered type : " + panelType);
                return;
            }
            var gameObject = Instantiate(prefab);
            if (gameObject == null)
            {
                Log.Error(MessageGroup.Common, "Cant instantiate gameobject panel");
                return;
            }

            DontDestroyOnLoad(gameObject);
            _instance._registeredPanels[panelType] = gameObject;
            gameObject.transform.SetLayer(gameObject);
            gameObject.transform.SetParent(_instance._canvasParent);
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