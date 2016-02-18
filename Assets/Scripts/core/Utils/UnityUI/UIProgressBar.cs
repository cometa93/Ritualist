using UnityEngine;
using UnityEngine.UI;

namespace DevilMind.Utils
{
    public class UIProgressBar : MonoBehaviour
    {
        [SerializeField] private Mask _mask;
        [SerializeField] private Image _progressBar;

        private float _value;
        public float Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
                if (_value < 0)
                {
                    _value = 0;
                }
                if (_value > 1)
                {
                    _value = 1;
                }
                UpdateProgressBar();
            } 
        }

        private void UpdateProgressBar()
        {
            var widthOfMask = _progressBar.rectTransform.rect.width;
            _mask.rectTransform.pivot = new Vector2(0, 0.5f);
 //           _mask.rectTransform.sizeDelta = new Vector2((widthOfMask * Value), _progressBar.rectTransform.rect.height);
            _mask.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, widthOfMask*Value);
        }
    }
}

