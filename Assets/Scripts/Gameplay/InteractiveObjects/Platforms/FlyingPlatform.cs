using System.Collections.Generic;
using DevilMind;
using Fading.Controller;
using UnityEngine;

namespace Fading.InteractiveObjects
{
    public class FlyingPlatform : DevilBehaviour
    {
        [SerializeField] private float _unitsPerSecond = 1f;
        [SerializeField] private List<Transform> _pointsToMoveWithin;
        [SerializeField] private Transform _platformTransform;
        [SerializeField] private Rigidbody2D _rigidbody2D;
        [SerializeField] private GameObject _transformToColor;
        [SerializeField] private Color _activeColor;
        [SerializeField] private Color _notActiveColor;

        private int _currentPointIndex = 1;
        private Transform _currentPointToMoveWithin;
        private bool _isActive;

        private bool IsActive
        {
            get
            {
                return _isActive;
            }
            set
            {
                if (_isActive == value)
                {
                    return;
                }
                _isActive = value;
                iTween.Stop(_transformToColor);
                _transformToColor.ColorTo(_isActive ? _activeColor : _notActiveColor, 0.5f, 0f);
            }   
        }
        private bool _reverting;

        protected override void Awake()
        {
            base.Awake();
            _currentPointToMoveWithin = _pointsToMoveWithin[_currentPointIndex];
        }

        protected override void Update()
        {
            if (IsActive == false)
            {
                return;
            }

            var maxDeltaDistance = _unitsPerSecond * Time.deltaTime;
            var position = Vector3.MoveTowards(_platformTransform.position, _currentPointToMoveWithin.position, maxDeltaDistance);
            _rigidbody2D.MovePosition(position);
            if (Vector2.Distance(_platformTransform.position, _currentPointToMoveWithin.position) < 0.02f)
            {
                ChangeCurrentPoint();
            }
        }

        private void ChangeCurrentPoint()
        {
            if (_reverting)
            {
                if (_currentPointIndex == 0)
                {
                    _reverting = false;
                    ChangeCurrentPoint();
                    return;
                }
                _currentPointIndex--;
                _currentPointToMoveWithin = _pointsToMoveWithin[_currentPointIndex];
            }
            else
            {
                if (_currentPointIndex == _pointsToMoveWithin.Count - 1)
                {
                    _reverting = true;
                    ChangeCurrentPoint();
                    return;
                }
                _currentPointIndex++;
                _currentPointToMoveWithin = _pointsToMoveWithin[_currentPointIndex];
            }
        }

        public void Activate()
        {
            IsActive = true;
        }

        public void Deactivate()
        {
            IsActive = false;
        }

  
    }
}