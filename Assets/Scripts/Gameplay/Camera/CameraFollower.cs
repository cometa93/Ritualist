using UnityEngine;
using System.Collections;
using DevilMind;

namespace Fading
{
    public class CameraFollower : DevilBehaviour
    {
        [SerializeField] private Transform _pointToFollow;
        [SerializeField] [Range(0.01f, 1f)] float _speedFactor;
        [SerializeField] private float _distanceMarginToStop;
        [SerializeField] private float _cameraZ;

        private Transform _myTransform;
        private Vector3 _lastPosition;

        protected override void Awake()
        {
            _myTransform = transform;
            base.Awake();
        }

        protected override void Update()
        {

            var temp = Vector3.Lerp(_myTransform.position, _pointToFollow.position * 1.2f, _speedFactor * Time.deltaTime);
            temp.z = _cameraZ;
            _myTransform.position = temp;
        }
    }
}

