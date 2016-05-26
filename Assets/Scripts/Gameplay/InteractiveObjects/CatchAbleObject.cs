using System.Collections;
using System.Collections.Generic;
using DevilMind;
using DevilMind.Utils;
using Fading.UI;
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
        protected bool IsAbleToCatch;

        [SerializeField] protected float EffectValue = 1f;
        [SerializeField] protected bool HasProgressBar;
        [SerializeField] protected List<CatchableObjectActivationField> MyTargetPoints;
        [SerializeField] private Transform _progressBarParent;
        [SerializeField] private GameObject _progressBarPrefab;

        private Coroutine _countingCoroutine;
        private ProgressBar _myProgressBar;
        protected override void Awake()
        {
            if (HasProgressBar)
            {
                var go = Instantiate(_progressBarPrefab);
                go.transform.SetLayer(_progressBarParent.gameObject);
                go.transform.SetParent(_progressBarParent);
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

        public virtual void ActivationFieldActivated()
        {
            bool isCatched = true;
            for (int i = 0, c = MyTargetPoints.Count; i < c; ++i)
            {
                var point = MyTargetPoints[i];
                if (point.IsActive == false)
                {
                    isCatched = false;
                }
            }

            IsCatched = isCatched;
        }
    }
}