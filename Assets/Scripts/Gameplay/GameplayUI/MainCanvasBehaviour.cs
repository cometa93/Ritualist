using System;
using System.Collections.Generic;
using DevilMind;
using DevilMind.Utils;
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

        private readonly List<UIType> _uiTypesThatCannotBeDeactivated = new List<UIType>
        {
            UIType.LoadingScreen
        };

        private readonly Dictionary<UIType,GameObject> _registeredPanels = new Dictionary<UIType, GameObject>();
        private readonly Dictionary<Type, UIType> _uiTypeToBehaviourType = new Dictionary<Type, UIType>
        {
            {typeof(GameplayMenuBehaviour),  UIType.GameplayMenu},
            {typeof(LoadingScreenBehaviour), UIType.LoadingScreen},
            {typeof(GameplayUserInterfaceBehaviour), UIType.GameplayGui}
        }; 

        public void Setup()
        {
            if (_instance != null)
            {
                Log.Warning(MessageGroup.Common, "Already registered main canvas");
                return;
            }
            _instance = this;
            CreatePanels();
            DontDestroyOnLoad(this);
        }

        private static void RegisterPanel(UIType panelType)
        {
            if (_instance._registeredPanels.ContainsKey(panelType))
            {
                Log.Warning(MessageGroup.Gui, "UI panel already registered type : " + panelType);
                return;
            }

            var prefab = GetPanelPrefab(panelType);
            if (prefab == null)
            {
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
            gameObject.transform.SetLayer(_instance.gameObject);
            gameObject.transform.SetParent(_instance._canvasParent,false);
            if (_instance._uiTypesThatCannotBeDeactivated.Contains(panelType) == false)
            {
                gameObject.SetActive(false);
            }
        }
        
        public static T GetUi<T>(UIType uiType = UIType.Unknown)
        {
            if (uiType == UIType.Unknown)
            {
                if (_instance._uiTypeToBehaviourType.TryGetValue(typeof(T), out uiType) == false)
                {
                    return default(T);
                }
            }

            GameObject panel;
            if (_instance._registeredPanels.TryGetValue(uiType, out panel) == false)
            {
                Log.Error(MessageGroup.Common, "There is no panel registered of type : " + uiType);
                return default(T);
            }

            var behaviour = panel.GetComponent<T>();
            if (behaviour == null)
            {
                Log.Error(MessageGroup.Common, "Couldn't get behaviour type of : " + typeof (T));
                return default(T);
            }

            return behaviour;
        }

        private static void DisablePanel(UIType panelType)
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

        private static void EnablePanel(UIType panelType)
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

        private static void DisableAllPanels()
        {
            for (int i = 1, c = (int) UIType.Count; i < c; ++i)
            {
                var uiType = (UIType) i;
                if (Instance._uiTypesThatCannotBeDeactivated.Contains(uiType))
                {
                    continue;
                }
                
                DisablePanel(uiType);
            }
        }

        private static GameObject GetPanelPrefab(UIType panelType)
        {
            return ResourceLoader.LoadUIPanel(panelType);
        }

        private static void CreatePanels()
        {
            for (int i = 1, c = (int) UIType.Count; i < c; ++i)
            {
                UIType type = (UIType) i;
                RegisterPanel(type);
            }
        }
        
        public static void EnablePanels(List<UIType> panels)
        {
            DisableAllPanels();
            for (int i = 0, c = panels.Count; i < c; ++i)
            {
                var type = panels[i];
                EnablePanel(type);
            }
        }

    }
}