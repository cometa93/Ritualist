using System;
using System.Collections;
using System.Collections.Generic;
using System.Security;
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

        private readonly Dictionary<int, string> _loadedStages = new Dictionary<int, string>(); 

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

        private void ShowLoadingScreen(Action onSceneShow)
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
                GameplayController.SetupCharacter();
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

            GameMaster.Events.Rise(EventType.StageLoaded);
            _loadingScreen.HideLoadingScreen(OnLoadingScreenHided);
        }

        private IEnumerator LoadSceneAsync(string scene, Action<float> onProgress, Action onLoaded, bool additive = false)
        {
            Application.backgroundLoadingPriority = ThreadPriority.Low;
            var async = SceneManager.LoadSceneAsync(scene, additive ? LoadSceneMode.Additive : LoadSceneMode.Single);
            async.allowSceneActivation = false;
            while (async.progress < 0.89f)
            {
                onProgress(async.progress);
                yield return new WaitForEndOfFrame();
            }
            
            async.allowSceneActivation = true;
            yield return new WaitForEndOfFrame();

            while (async.isDone == false)
            {
                onProgress(async.progress);
                yield return new WaitForEndOfFrame();
            }

            onLoaded();
            Application.backgroundLoadingPriority = ThreadPriority.BelowNormal;
        }

        #region Stages Scene Managment

        public void StageLoaded()
        {
            _isLoadingStage = false;
            CheckLoadedObjectsState();
        }

        public void LoadStage(int number)
        {
            if (_loadedStages.ContainsKey(number))
            {
                return;
            }
            
            _currentScene = GameSceneType.Gameplay;
            _isLoadingStage = true;
            _loadedStages[number] = GetStageName(number);
            StartCoroutine(LoadSceneAsync(GetStageName(number), OnProgress, OnSceneLoaded, true));
        }

        public void UnloadStage(int number)
        {
            string stageName = "";
            if (_loadedStages.TryGetValue(number, out stageName) == false)
            {
                return;
            }

            SceneManager.UnloadScene(stageName);
            _loadedStages.Remove(number);
        }

        public void LoadStages(List<int> stageNumbersToLoad)
        {
            for (int i = 0, c = stageNumbersToLoad.Count; i < c; ++i)
            {
                var number = stageNumbersToLoad[i];
                LoadStage(number);
            }

            foreach (var stage in _loadedStages)
            {
                if (stageNumbersToLoad.Contains(stage.Key))
                {
                    continue;
                }

                UnloadStage(stage.Key);
            }
        }

        public void LoadClearStage(int stageNumber)
        {
            _currentScene = GameSceneType.Gameplay;
            GameplayController.DestroyGameplayController();
            GameplayController.CreateGameplayController();
            _loadedStages.Clear();
            _loadedStages[stageNumber] = GetStageName(stageNumber);

            _isLoadingStage = true;
            ShowLoadingScreen(() =>
            {
                StartCoroutine(LoadSceneAsync(GetStageName(stageNumber), OnProgress, () =>
                {
                    OnSceneLoaded();
                    GameplayController.SetupCharacter();
                }));
            });
        }

        private string GetStageName(int number)
        {
            return number + StagePrefix;
        }

        #endregion

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

            GameplayController.DestroyGameplayController();
            _currentScene = type;
            string sceneName;
            if (_sceneBuildNames.TryGetValue(_currentScene, out sceneName))
            {

                ShowLoadingScreen(() =>
                {
                    _isLoadingStage = false;
                    StartCoroutine(LoadSceneAsync(sceneName, OnProgress, OnSceneLoaded));
                });
            }
        }

     
    }
}