using System;
using System.Collections;
using DevilMind;
using UnityEngine;
using Event = DevilMind.Event;
using EventType = DevilMind.EventType;

namespace Fading.UI
{
    public class GameplayMenuBehaviour : DevilBehaviour
    {
        public float AnimationTime = 0.5f;

        [SerializeField] private CanvasRenderer _characterDiedMenu;
        [SerializeField] private CanvasRenderer _pauseMenu;
        
        private Coroutine _animationCoroutine;
        private CanvasRenderer _animatedRenderer;

        protected override void Start()
        {
            _characterDiedMenu.SetAlpha(0);
            _characterDiedMenu.gameObject.SetActive(false);
            _pauseMenu.SetAlpha(0);
            _pauseMenu.gameObject.SetActive(false);

            base.Start();
        }

        protected override void Awake()
        {
            EventsToListen.Add(EventType.CharacterDied);
            EventsToListen.Add(EventType.PauseGame);
            base.Awake();
        }

        protected override void OnEvent(Event gameEvent)
        {
            switch (gameEvent.Type)
            {
                case EventType.CharacterDied:
                    ShowDiedMenu();
                    break;

                case EventType.PauseGame:
                    if (gameEvent.Parameter == null)
                    {
                        Log.Error(MessageGroup.Gameplay, "Pause event was rised without bool parameter");
                        break;
                    }

                    var arg = (bool) gameEvent.Parameter;
                    if (arg)
                    {
                        ShowPauseMenu();
                    }
                    else
                    {
                        HidePauseMenu();
                    }
                    break;
            }
            base.OnEvent(gameEvent);
        }

        private void ShowDiedMenu()
        {
            StopAnimation();
            _characterDiedMenu.gameObject.SetActive(true);
            _characterDiedMenu.SetAlpha(0f);
            _animationCoroutine = StartCoroutine(AnimateFade(_characterDiedMenu));
        }

        private void StopAnimation()
        {
            if (_animationCoroutine != null)
            {
                StopCoroutine(_animationCoroutine);
            }
            if (_animatedRenderer != null)
            {
                _animatedRenderer.SetAlpha(0f);
                _animatedRenderer = null;
            }

            _pauseMenu.gameObject.SetActive(false);
            _characterDiedMenu.gameObject.SetActive(false);
        }

        private void ShowPauseMenu()
        {
            StopAnimation();
            _pauseMenu.gameObject.SetActive(true);
            _pauseMenu.SetAlpha(0f);
            _animationCoroutine = StartCoroutine(AnimateFade(_pauseMenu));
        }

        private void HideDiedMenu()
        {
            StopAnimation();
            _animationCoroutine = StartCoroutine(AnimateFade(_characterDiedMenu,true, () =>
            {
                _characterDiedMenu.gameObject.SetActive(false);
                _characterDiedMenu.SetAlpha(0f);
            }));
        }

        private void HidePauseMenu()
        {
            StopAnimation();
            _animationCoroutine = StartCoroutine(AnimateFade(_pauseMenu, true, () =>
            {
                _pauseMenu.gameObject.SetActive(false);
                _pauseMenu.SetAlpha(0f);
            }));
        }

        private IEnumerator AnimateFade(
            CanvasRenderer rendererToFade,
            bool backward = false,
            Action onFinish = null)
        {

            var alphaToFade = backward ? rendererToFade.GetAlpha() : 1 - rendererToFade.GetAlpha();
            float time = alphaToFade * AnimationTime;
            float currentTime = 0;
            while (currentTime < time)
            {
                currentTime += Time.unscaledDeltaTime;
                float currentAlpha = Mathf.Lerp(backward ? rendererToFade.GetAlpha() : 0, backward ? 0 : 1f,
                    currentTime/time);
                rendererToFade.SetAlpha(currentAlpha);
                yield return null;
            }

            if (onFinish != null && enabled)
            {
                onFinish();
            }
        }

        public void OnClickLoadGame()
        {
            GameMaster.GameSave.LoadCurrentGame();
            HideDiedMenu();
        }
    }
}