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
        public int CurrentStage { private set; get; }
        private LoadingScreenBehaviour _loadingScren;
        private static SceneLoader _instance;
        public static SceneLoader Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject go = new GameObject("Scene Loader");
                    var loader = go.AddComponent<SceneLoader>();
                    _instance = loader;
                    _instance.CreateLoadingScreen();
                }
                return _instance;
            }
        }

        private void Start()
        {
            DontDestroyOnLoad(gameObject);
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
            _loadingScren.HideLoadingScreen(OnLoadingScreenHided);
        }

        public void LoadStage(int number)
        {
            ShowLoadingStageScene(() =>
            {
                CurrentStage = number;
                StartCoroutine(LoadSceneAsync(number + StagePrefix, OnProgress, OnSceneLoaded));
            });
        }

    }
}