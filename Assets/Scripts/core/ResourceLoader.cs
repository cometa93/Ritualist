﻿using System.Collections.Generic;
using DevilMind.Utils;
using UnityEngine;

namespace DevilMind
{
    public static class ResourceLoader
    {
        private const string _prefabsFolder = "Prefabs/";
        private const string _enemiesFolder = "Enemies/";
        private const string _buttonsPath = "Buttons/";

        private static readonly Dictionary<ButtonType,string> ButtonsPaths =  new Dictionary<ButtonType, string>
        {
            {ButtonType.Standard, "StandardButton"},
            {ButtonType.Exit, "ExitButton"},
        };

        private static readonly Dictionary<EnemyType,string> Enemies = new Dictionary<EnemyType, string>()
        {
            {EnemyType.Ghost,  "Ghost"},
        }; 
        
        public static void SetLayer(Transform t, int layer)
        {
            if (t == null)
            {
                return;
            }

            t.gameObject.layer = layer;

            foreach (Transform child in t)
            {
                SetLayer(child, layer);
            }
        }

        #region Loading Helpers

        public static GameObject LoadButton(ButtonType type)
        {
            if (ButtonsPaths.ContainsKey(type) == false)
            {
                return null; 
            }

            var buttonName = ButtonsPaths[type];
            return Load<GameObject>(_prefabsFolder +_buttonsPath + buttonName);
        }

        public static GameObject LoadEnemy(EnemyType enemy)
        {
            if (Enemies.ContainsKey(enemy) == false)
            {
                Debug.Log("error with loading enemy");
                return null;
            }
            return Load<GameObject>(_enemiesFolder + Enemies[enemy]);
        }
        #endregion

        private static T Load<T>(string name) where T : Object
        {
            Object loadedObject = Resources.Load(name, typeof(T));
            if (loadedObject == null )
            {
                Log.Warning(MessageGroup.Common, "Couldn't load object with the name : " + name);
                return null;
            }

            if (loadedObject.GetType() != typeof (T))
            {
                Log.Warning(MessageGroup.Common, "Loaded object with name : " + name + " cannot be converted to type:  " + typeof(T));
                return null;
            }

            return loadedObject as T;
        }

  
    }
}