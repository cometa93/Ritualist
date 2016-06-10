using System.Runtime.InteropServices;
using Fading.Controller;
using Fading.UI;
using SRF;
using UnityEngine;

namespace DevilMind
{
    public class GameMasterBehaviour : MonoBehaviour
    {
        private static bool _isInstanceCreated = false;
        
        [RuntimeInitializeOnLoadMethod]
        private static void GenerateGameMasterOnAppStart()
        {
            if (_isInstanceCreated == false)
            {
                var go = new GameObject("GameMaster(DONTDESTROY)");
                go.transform.SetAsFirstSibling();
                go.AddComponent<GameMasterBehaviour>();
            }
        }

        void Awake()
        {
            _isInstanceCreated = true;
            DontDestroyOnLoad(this);
            CreateMainCanvas();
            CreateSceneLoader();
        }

        private void CreateSceneLoader()
        {
            if (SceneLoader.Instance == null)
            {
                var go = new GameObject("SceneLoader (DONT DESTROY)");
                go.transform.SetSiblingIndex(1);
                go.AddComponent<SceneLoader>();
            }
        }

        private void CreateMainCanvas()
        {
            if (MainCanvasBehaviour.Instance == null)
            {
                var go = ResourceLoader.LoadMainCanvas();
                if (go == null)
                {
                    Log.Error(MessageGroup.Common, "Cannot create main canvas object prefab is null");
                    return;
                }
                var spawned = Instantiate(go);
                if (spawned == null)
                {
                    Log.Error(MessageGroup.Common, "Spawned main canvas is null");
                    return;
                }

                spawned.transform.ResetLocals();
                spawned.name = "MainCanvas (DONT DESTROY)";
                var canvasBehaviour = spawned.GetComponent<MainCanvasBehaviour>();
                canvasBehaviour.Setup();
            }
        }

        public void Update()
        {
            GameMaster.Instance.MainLoop(Time.deltaTime);
            MyInputManager.Update();
        }

    }
}