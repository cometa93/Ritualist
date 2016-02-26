using System.Collections;
using DevilMind;
using Ritualist.UI;
using UnityEngine;

namespace Assets.Scripts.Gameplay.InteractiveObjects
{
    public class CatchAbleObject : DevilBehaviour
    {
        protected const string IsCatchedAnimatorBool = "IsCatched";
        protected const string IsFinished = "IsFinished";

        protected float TimeToCatch = 1f;
        private float Value;
        private bool _isCatched;
        private bool _isFinished;
        protected bool _isAbleToCatch;
        [SerializeField] protected float EffectValue = 1f;
        [SerializeField] protected bool HasProgressBar;
        [SerializeField] private Transform _progressBarParent;
        [SerializeField] private GameObject _progressBarPrefab;

        private Coroutine _countingCoroutine;
        private ProgressBar _myProgressBar;
        protected override void Awake()
        {
            if (HasProgressBar)
            {
                var go = Instantiate(_progressBarPrefab);
                ResourceLoader.SetLayer(go.transform, gameObject.layer);
                go.transform.parent = _progressBarParent;
                go.transform.localScale = Vector3.one;
                go.transform.localPosition = Vector3.zero;
                _myProgressBar = go.GetComponent<ProgressBar>();
            }

            base.Awake();
        }

        public bool IsCatched
        {
            get
            {
                return _isCatched;
            }
            set
            {
                if (_isFinished)
                {
                    return;
                }
                if (value == _isCatched)
                {
                    return;
                }
                _isCatched = value;

                Value = 0f;
                if (value)
                {
                    if (_countingCoroutine != null)
                    {
                        StopCoroutine(_countingCoroutine);
                        _countingCoroutine = null;
                    }

                    OnCatched();
                    _countingCoroutine = StartCoroutine(StartCounting());
                }
                else
                {
                    if (_countingCoroutine != null)
                    {
                        StopCoroutine(_countingCoroutine);
                        _countingCoroutine = null;
                    }
                    OnRelease();
                }
            }
        }

        public float Progress
        {
            get { return Mathf.Clamp(Value/TimeToCatch, 0, 1f); }
        }

        IEnumerator StartCounting()
        {
            while (Progress < 0.99f)
            {
                OnProgressChanged();
                Value += Time.deltaTime;   
                yield return null;
            }
            _isFinished = true;
            OnFinished();
        }
        
        protected virtual void OnProgressChanged()
        {
            if (HasProgressBar && _myProgressBar != null)
            {
                _myProgressBar.Value = Mathf.Clamp(1f - Progress, 0, 1);
            }
        }

        protected virtual void OnCatched()
        {
        }

        protected virtual void OnFinished()
        {
        }

        protected virtual void OnRelease()
        {
        }

    }
}