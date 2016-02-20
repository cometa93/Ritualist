using System.Collections;
using DevilMind;
using UnityEngine;

namespace Assets.Scripts.Gameplay.InteractiveObjects
{
    public class CatchAbleObject : DevilBehaviour
    {
        protected float TimeToCatch = 1f;
        private float Value;
        private bool _isCatched;
        private bool _isFinished;

        private Coroutine _countingCoroutine;

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
        }


        protected virtual void OnProgressChanged()
        {
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