using System.Runtime.InteropServices;
using Fading.Controller;
using Fading.UI;
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
                GameMasterBehaviour gm = go.AddComponent<GameMasterBehaviour>();
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
                var go = new GameObject("SceneLoader(DONTDESTROY)");
                go = Instantiate(go);
                go.AddComponent<SceneLoader>();
            }
        }

        private void CreateMainCanvas()
        {
            if (MainCanvasBehaviour.Instance == null)
            {
                var go = new GameObject("MainCanvas(DONTDESTROY)");
                var canvasBehaviour = go.AddComponent<MainCanvasBehaviour>();
                canvasBehaviour.Register();
            }
        }

        public void Update()
        {
            GameMaster.Instance.MainLoop(Time.deltaTime);
            MyInputManager.Update();
        }

    }
}