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
        private const string StagePrefix = "_STAGE";

        private static SceneLoader _instance;
        private static int _objectsToLoadStateRefCounter;
        private bool _isLoadingStage;
        private LoadingScreenBehaviour _loadingScreen;
        private GameSceneType _currentScene = GameSceneType.Unknown;

        private readonly Dictionary<GameSceneType, string> _sceneBuildNames = new Dictionary<GameSceneType, string>
        {
            { GameSceneType.MainMenu, "MainMenu"}
        };

        private readonly Dictionary<GameSceneType, List<UIType>> _uiTypesEnabledOnGameSceneTypes = new Dictionary<GameSceneType, List<UIType>>
        {
            {GameSceneType.Gameplay, new List<UIType> {UIType.GameplayGui, UIType.GameplayMenu}},
            {GameSceneType.MainMenu, new List<UIType>()}
        };

        public static int ObjectsToLoadStateRefCounter
        {
            get { return _objectsToLoadStateRefCounter; }
            set
            {
                _objectsToLoadStateRefCounter = value;
                Instance.CheckLoadedObjectsState();
            }
        }
        
        public bool IsOnStage
        {
            get { return _currentScene == GameSceneType.Gameplay; }
        }

        public int CurrentStage { private set; get; }

        public static SceneLoader Instance
        {
            get
            {
                return _instance;
            }
        }
        
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
        
        private void Awake()
        {
            _instance = this;
            DontDestroyOnLoad(this);
        }

        private void CreateLoadingScreen()
        {
            if (_loadingScreen != null)
            {
                return;
            }

            _loadingScreen = MainCanvasBehaviour.GetUi<LoadingScreenBehaviour>();
        }

        private void ShowLoadingStageScene(Action onSceneShow)
        {
            LoadingScreen.ShowLoadingScreen(onSceneShow);
        }

        private void OnSceneLoaded()
        {
            List<UIType> panelTypesToEnable;
            if (_uiTypesEnabledOnGameSceneTypes.TryGetValue(_currentScene, out panelTypesToEnable))
            {
                MainCanvasBehaviour.EnablePanels(panelTypesToEnable);
            }

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

        private void CheckLoadedObjectsState()
        {
            if (_objectsToLoadStateRefCounter > 0 ||
                _isLoadingStage)
            {
                return;
            }

            _loadingScreen.HideLoadingScreen(OnLoadingScreenHided);
        }

        private IEnumerator LoadSceneAsync(string scene, Action<float> onProgress, Action onLoaded)
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
            CheckLoadedObjectsState();
        }

        public void LoadStage(int number)
        {
            _currentScene = GameSceneType.Gameplay;
            ShowLoadingStageScene(() =>
            {
                _isLoadingStage = true;
                CurrentStage = number;
                StartCoroutine(LoadSceneAsync(number + StagePrefix, OnProgress, OnSceneLoaded));
            });
        }
        
        public void LoadScene(GameSceneType type)
        {
            if (_currentScene == type)
            {
                return;
            }

            if (type == GameSceneType.Gameplay)
            {
                Log.Error(MessageGroup.Common, "Wrong usage of function stage should be loaded by LoadStage function");
                return;
            }

            _currentScene = type;
            string sceneName;
            if (_sceneBuildNames.TryGetValue(_currentScene, out sceneName))
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