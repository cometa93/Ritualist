using DevilMind;
using UnityEngine;

namespace Fading.UI
{
    public class LoadingScreenBehaviour : MonoBehaviour
    {
        private System.Action _onLoadingScreenViewed;
        private System.Action _onLoadingScreenHide;
        private bool _isVisible;

#pragma warning disable 649
        [SerializeField] private Animator _animator;
#pragma warning restore 649

        public void Awake()
        {
            DontDestroyOnLoad(gameObject);
            _isVisible = false;
        }

        public void OnLoadingScreenViewed()
        {
            if (_onLoadingScreenViewed != null)
            {
                _onLoadingScreenViewed();
            }
            _onLoadingScreenViewed = null;
        }

        public void OnLoadingScreenHided()
        {
            if (_onLoadingScreenHide != null)
            {
                _onLoadingScreenHide();
            }
            _onLoadingScreenHide = null;
        }

        public void ShowLoadingScreen(System.Action onViewed)
        {
            if (_isVisible)
            {
                if (onViewed != null)
                {
                    onViewed();
                }
                return;
            }

            _isVisible = true;
            _onLoadingScreenViewed = onViewed;
            _animator.SetTrigger("Show");
        }

        public void HideLoadingScreen(System.Action onHide)
        {
            if (_isVisible == false)
            {
                if (onHide != null)
                {
                    onHide();
                }
                return;
            }

            _isVisible = false;
            _onLoadingScreenHide = onHide;
            _animator.SetTrigger("Hide");
        }
    }
}