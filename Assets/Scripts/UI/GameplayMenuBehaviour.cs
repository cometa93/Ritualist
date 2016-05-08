using System.Collections;
using DevilMind;
using UnityEngine;
using Event = DevilMind.Event;
using EventType = DevilMind.EventType;

namespace Fading.UI
{
    public class GameplayMenuBehaviour : DevilBehaviour
    {
        public static bool GameplayMenuCreated = false;
        [SerializeField] private CanvasRenderer _characterDiedMenu;

        protected override void Start()
        {
            _characterDiedMenu.SetAlpha(0);
            _characterDiedMenu.gameObject.SetActive(false);
            DontDestroyOnLoad(this);
            GameplayMenuCreated = true;
            base.Start();
        }

        protected override void Awake()
        {
            EventsToListen.Add(EventType.CharacterDied);
            base.Awake();
        }

        protected override void OnEvent(Event gameEvent)
        {
            switch (gameEvent.Type)
            {
                    case EventType.CharacterDied:
                        ShowDiedMenu();
                        break;
            }
            base.OnEvent(gameEvent);
        }

        private void ShowDiedMenu()
        {
            _characterDiedMenu.gameObject.SetActive(true);
            _characterDiedMenu.SetAlpha(0f);
            StartCoroutine(AnimateFade());
        }

        private void HideDiedMenu()
        {
            StartCoroutine(AnimateFade(true, () =>
            {
                _characterDiedMenu.gameObject.SetActive(false);
                _characterDiedMenu.SetAlpha(0f);
            }));
        }

        private IEnumerator AnimateFade(bool backward = false, System.Action onFinish = null)
        {
            float time = 1f;
            float currentTime = 0;
            while (currentTime < time)
            {
                currentTime += Time.unscaledDeltaTime;
                var progress = backward ? 1 - Mathf.Clamp01(currentTime/time) : Mathf.Clamp01(currentTime / time);
                _characterDiedMenu.SetAlpha(progress);
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