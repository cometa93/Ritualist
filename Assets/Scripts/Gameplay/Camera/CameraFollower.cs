using System;
using UnityEngine;
using System.Collections;
using DevilMind;
using Event = DevilMind.Event;
using EventType = DevilMind.EventType;

namespace Fading
{
    public class CameraFollower : DevilBehaviour
    {
        [SerializeField] [Range(0f, 1f)] private float _overtakeMultiplier;
        [SerializeField] private Transform _goodSoulTransform;
        [SerializeField] private Transform _charTransform;
        [SerializeField] private float _minCameraZ;
        [SerializeField] private float _maxCameraZ;
        [SerializeField] private float _objectMaxSpeed = 15f;
        [SerializeField] [Range(0f, 15f)] private float _offsetDepthStartingMove;

        private Transform _myTransform;
        private Transform _objectToFollow;
        private Rigidbody2D _possibleRigidbody2D;
        private bool _isCharacterNow = true;

        
        protected override void Awake()
        {
            _myTransform = transform;
            SetObjectToFollow(_charTransform);
            EventsToListen.Add(EventType.CharacterChanged);
            base.Awake();
        }

        protected override void OnEvent(Event gameEvent)
        {
            switch (gameEvent.Type)
            {
                case EventType.CharacterChanged:
                    _isCharacterNow = !_isCharacterNow;
                    SetObjectToFollow(_isCharacterNow ? _charTransform : _goodSoulTransform);
                    break;
            }
            base.OnEvent(gameEvent);
        }

        protected override void Update()
        {
            if (_possibleRigidbody2D != null)
            {
                var position = Vector3.Lerp(_myTransform.position, _objectToFollow.position + new Vector3(_possibleRigidbody2D.velocity.x, _possibleRigidbody2D.velocity.y) *_overtakeMultiplier*2.5f, Time.deltaTime*0.7f);
                var velocityLenght = _possibleRigidbody2D.velocity.magnitude;
                var calculatedZ = _minCameraZ;
                if (velocityLenght >= _offsetDepthStartingMove)
                {
                    var percent = Mathf.Clamp01(velocityLenght/_objectMaxSpeed);
                    calculatedZ = Mathf.Lerp(_minCameraZ, _maxCameraZ, percent);
                    position.z = Mathf.Lerp(_myTransform.position.z, calculatedZ, Time.deltaTime * 0.4f);
                    _myTransform.position = position;
                    return;
                }

                position.z = Mathf.Lerp(_myTransform.position.z, calculatedZ, Time.deltaTime * 0.2f);
                _myTransform.position = position;
                return;
            }

            var pose = Vector3.Lerp(_myTransform.position, _objectToFollow.position, Time.deltaTime);
            pose.z = _minCameraZ;
            _myTransform.position = pose;
        }

        private void SetObjectToFollow(Transform t)
        {
            _objectToFollow = t;
            _possibleRigidbody2D = t.GetComponent<Rigidbody2D>();
        }
    }
}

