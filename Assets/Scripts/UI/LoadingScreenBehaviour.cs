using DevilMind;
using UnityEngine;

namespace Ritualist.UI
{
    public class LoadingScreenBehaviour : MonoBehaviour
    {
        private System.Action _onLoadingScreenViewed;
        private System.Action _onLoadingScreenHide;

        [SerializeField] private Animator _animator;
        public void Awake()
        {
            DontDestroyOnLoad(gameObject);
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
            _onLoadingScreenViewed = onViewed;
            _animator.SetTrigger("Show");
        }

        public void HideLoadingScreen(System.Action onHide)
        {
            _onLoadingScreenHide = onHide;
            _animator.SetTrigger("Hide");
        }
    }
}