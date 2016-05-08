using System.Runtime.InteropServices;
using Fading.Controller;
using UnityEngine;

namespace DevilMind
{
    public class GameMasterBehaviour : MonoBehaviour
    {
        void Start()
        {
            DontDestroyOnLoad(this);
            CreateSceneLoader();
        }

        private void CreateSceneLoader()
        {
            if (SceneLoader.Instance == null)
            {
                var go = new GameObject("SceneLoader");
                go = Instantiate(go);
                go.AddComponent<SceneLoader>();
            }
        }

        public void Update()
        {
            GameMaster.Instance.MainLoop(Time.deltaTime);
            MyInputManager.Update();
        }

    }
}