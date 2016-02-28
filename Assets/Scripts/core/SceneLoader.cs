using System;
using System.Collections;
using System.Collections.Generic;
using Ritualist.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DevilMind
{
    public class SceneLoader : MonoBehaviour
    {

        private const string StagePrefix = "_STAGE";
        private bool _isLoadingStage;
        //TODO MAKE IT FROM INITIALIZATION FUNCTION WHICH WILL PROVIDE TO STARTING SCENE FROM SPLASHSCREEN
        private string _currentScene = "1_STAGE";
        private LoadingScreenBehaviour _loadingScren;
        private static SceneLoader _instance;
        public static SceneLoader Instance
        {
            get { return _instance; }
        }

        private void Awake()
        {
            _instance = this;
        }

        private void Start()
        {
            DontDestroyOnLoad(gameObject);
            CreateLoadingScreen();
        }

        private void CreateLoadingScreen()
        {
            if (_loadingScren != null)
            {
                return;
            }

            var go = ResourceLoader.LoadLoadingScreen();
            if (go == null)
            {
                Log.Error(MessageGroup.Gameplay, "Cant load loading screen prefab");
                return;
            }

            go = Instantiate(go);
            if (go == null)
            {
                Log.Error(MessageGroup.Gameplay, "Cant instantiate loading screen prefab");
                return;
            }

            _loadingScren = go.GetComponent<LoadingScreenBehaviour>();
            if (_loadingScren == null)
            {
                Log.Error(MessageGroup.Gameplay, "Loading screen dont have behaviour");
            }
        }

        private void ShowLoadingStageScene(Action onSceneShow)
        {
            _loadingScren.ShowLoadingScreen(onSceneShow);
        }

        private void OnSceneLoaded()
        {
            if (_isLoadingStage)
            {
                return;
            }

            _loadingScren.HideLoadingScreen(OnLoadingScreenHided);
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
//            Find other options to see if 
//            if (string.IsNullOrEmpty(_currentScene) == false)
//            {
//                SceneManager.UnloadScene(_currentScene);
//            }
            _currentScene = scene;
            async.allowSceneActivation = true;

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
            _loadingScren.HideLoadingScreen(OnLoadingScreenHided);
        }

        public void LoadStage(int number)
        {
            ShowLoadingStageScene(() =>
            {
                StartCoroutine(LoadSceneAsync(number + StagePrefix, OnProgress, OnSceneLoaded));
            });
        }

    }
}