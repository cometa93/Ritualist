using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fading.UI
{
    public class ProgressBar : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _foreground;
        [SerializeField] private SpriteRenderer _background;
        [SerializeField] private Color _foregroundInitialColor;
        [SerializeField] private Color _foregroundFinalColor;
        [SerializeField] private float _timeTillFadeOut;
        [SerializeField] private float _fadeOutDuration;


        private float _foregroundAlpha;
        private float _backgroundAlpha;
        private float _currentForegroundAlpha;
        private float _currentBackgroundAlpha;
        private bool _isActive;
        private float _fadeOutProgress;

        private float _value;

        private Coroutine _fadeOutCoroutine;

        public float Value
        {
            set
            {
                _value = value;
                if (_fadeOutCoroutine != null)
                {
                    StopCoroutine(_fadeOutCoroutine);
                    _fadeOutCoroutine = null;
                }

                _isActive = true;
                var bgColor = _background.color;
                var fgColor = _foreground.color;
                bgColor.a = _backgroundAlpha;
                fgColor.a = _foregroundAlpha;
                _background.color = bgColor;
                _foreground.color = fgColor;
                _fadeOutCoroutine = StartCoroutine(FadeOut());
            }
        }

        private void Awake()
        {
            _foregroundAlpha = _foreground.color.a;
            _backgroundAlpha = _background.color.a;
        }

        private void Start()
        {
            _currentBackgroundAlpha = 0;
            _currentForegroundAlpha = 0;
            _background.color = new Color(_background.color.r, _background.color.g, _background.color.b,_currentBackgroundAlpha);
            _foreground.color = new Color(_foreground.color.r, _foreground.color.g, _foreground.color.b, _currentForegroundAlpha);
        }

        private void Update()
        {
            if (_isActive == false)
            {
                return;
            }

            var scale = _foreground.transform.localScale;
            scale.x = Mathf.Lerp(_foreground.transform.localScale.x, _value, Time.unscaledDeltaTime);
            _foreground.transform.localScale = scale;
        }

        IEnumerator FadeOut()
        {
            yield return new WaitForSeconds(1f);

            float progress = 0f;
            while (progress < 1f)
            {
                _fadeOutProgress += Time.unscaledDeltaTime;
                progress = _fadeOutProgress / _fadeOutDuration;
                _currentBackgroundAlpha = Mathf.Lerp(_backgroundAlpha, 0, progress);
                _currentForegroundAlpha = Mathf.Lerp(_foregroundAlpha, 0, progress);

                var bgColor = _background.color;
                var fgColor = _foreground.color;
                bgColor.a = _currentBackgroundAlpha;
                fgColor.a = _currentForegroundAlpha;
                _background.color = bgColor;
                _foreground.color = fgColor;
                yield return null;
            }

            _isActive = false;
        }

    }
}