using System.Collections;
using System.Security.Permissions;
using UnityEngine;
using UnityEngine.UI;
using DevilMind;
using UnityEngine.SceneManagement;
using Event = DevilMind.Event;
using EventType = DevilMind.EventType;

namespace Ritualist.UI
{
    public class CounterBehaviour : DevilBehaviour
    {
        [SerializeField] private Text text;
        [SerializeField] private Image _image;
        [SerializeField] private Image _panel;

        protected override void Awake()
        {
            _panel.color = Color.clear;
            _image.color = Color.clear;
            EventsToListen.Add(EventType.EnemyDied);
            EventsToListen.Add(EventType.GameEnd);
            base.Awake();
        }

        protected override void OnEvent(Event gameEvent)
        {
            if (gameEvent.Type == EventType.EnemyDied)
            {
                UpdateCounter();
            }
            if (gameEvent.Type == EventType.GameEnd)
            {
              EndGame();
            }
            base.OnEvent(gameEvent);
        }

        private void UpdateCounter()
        {
            text.text = GameplayController.Instance.EnemyKilledCounter.ToString();
        }

        private void EndGame()
        {
            GameplayController.Instance.AmmoCount = 3;
            GameplayController.Instance.EnemyKilledCounter = 0;
            StartCoroutine(FadeColor());
            StartCoroutine(WaitAWhile());
        }

        private IEnumerator FadeColor()
        {
            float time = 0.5f;
            float counter = 0;
            var init = Color.clear;
            var end = new Color(0, 0, 0, 0.6f);
            var end2 = new Color(1, 1, 1, 1);
            while (counter < time)
            {
                yield return null;
                counter += Time.deltaTime;
                float progress = counter/time;
                _panel.color = Color.Lerp(init, end, progress);
                _image.color = Color.Lerp(init, end2, progress);
            }
        }

        private IEnumerator WaitAWhile()
        {
            yield return new WaitForSeconds(3f);
            SceneManager.LoadScene("SplashScreen");
        }
    }
}

