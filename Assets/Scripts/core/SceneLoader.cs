using System;
using System.Collections;
using System.Collections.Generic;
using Fading;
using Fading.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DevilMind
{
    public class SceneLoader : MonoBehaviour
    {
        //TODO ADD VALIDATION IF IT IS GAMEPLAY SCENE OR OTHER
        public bool IsOnStage
        {
            get
            {
                var sceneIndex = SceneManager.GetActiveScene().buildIndex;
                return  sceneIndex > 1; 
            }
        }

        private readonly Dictionary<GameSceneType, string> SceneBuildNames= new Dictionary<GameSceneType, string>
        {
            { GameSceneType.MainMenu, "MainMenu"}
        };

        private GameSceneType CurrentScene = GameSceneType.Unknown;

        private const string StagePrefix = "_STAGE";
        private bool _isLoadingStage;
        public int CurrentStage { private set; get; }

        private LoadingScreenBehaviour _loadingScreen;
        private LoadingScreenBehaviour LoadingScreen
        {
            get
            {
                if (_instance._loadingScreen == null)
                {
                    CreateLoadingScreen();
                }
                return _loadingScreen;
            }
        }

        private static SceneLoader _instance;

        private void Awake()
        {
            _instance = this;
            DontDestroyOnLoad(this);
        }

        public static SceneLoader Instance
        {
            get
            {
                return _instance;
            }
        }

        private void CreateLoadingScreen()
        {
            if (_loadingScreen != null)
            {
                return;
            }

            var go = ResourceLoader.LoadLoadingScreen();
            if (go == null)
            {
                Log.Error(MessageGroup.Gameplay, "Cant load loading screen prefab");
                return;
            }

            go = MainCanvasBehaviour.RegisterPanel(UIType.LoadingScreen, go);
            if (go == null)
            {
                return;
            }

            _loadingScreen = go.GetComponent<LoadingScreenBehaviour>();
            if (_loadingScreen == null)
            {
                Log.Error(MessageGroup.Gameplay, "Loading screen dont have behaviour");
            }

#if UNITY_EDITOR
            // Only for initialization of gameplay controller on first scene while testing.
            if (IsOnStage)
            {
                GameplayController.CreateGameplayControllerOnStageLoaded();
            }
#endif
        }

        private void ShowLoadingStageScene(Action onSceneShow)
        {
            LoadingScreen.ShowLoadingScreen(onSceneShow);
        }

        private void OnSceneLoaded()
        {
            if (_isLoadingStage)
            {
                GameplayController.CreateGameplayControllerOnStageLoaded();
                return;
            }

            LoadingScreen.HideLoadingScreen(OnLoadingScreenHided);
        }

        private void OnLoadingScreenHided()
        {
        }

        private void OnProgress(float progress)
        {
        }

        private IEnumerator LoadSceneAsync(string scene, System.Action<float> onProgress, System.Action onLoaded)
        {
            var async = SceneManager.LoadSceneAsync(scene);
            async.allowSceneActivation = false;
            while (async.progress < 0.89f)
            {
                onProgress(async.progress);
                yield return null;
            }

            async.allowSceneActivation = true;
            yield return new WaitForSeconds(0.5f);

            while (async.isDone == false)
            {
                onProgress(async.progress);
                yield return null;
            }

            onLoaded();
        }

        public void StageLoaded()
        {
            _isLoadingStage = false;
            LoadingScreen.HideLoadingScreen(OnLoadingScreenHided);
        }

        public void LoadStage(int number)
        {
            CurrentScene = GameSceneType.Gameplay;
            ShowLoadingStageScene(() =>
            {
                _isLoadingStage = true;
                CurrentStage = number;
                StartCoroutine(LoadSceneAsync(number + StagePrefix, OnProgress, OnSceneLoaded));
            });
        }

        public void LoadScene(GameSceneType type)
        {
            if (CurrentScene == type)
            {
                return;
            }

            if (type == GameSceneType.Gameplay)
            {
                Log.Error(MessageGroup.Common, "Wrong usage of function stage should be loaded by LoadStage function");
                return;
            }

            CurrentScene = type;
            string sceneName;
            if (SceneBuildNames.TryGetValue(CurrentScene, out sceneName))
            {

                ShowLoadingStageScene(() =>
                {
                    _isLoadingStage = false;
                    CurrentStage = 0;
                    StartCoroutine(LoadSceneAsync(sceneName, OnProgress, OnSceneLoaded));
                });
            }
        }

    }
}